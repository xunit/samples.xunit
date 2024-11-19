using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace NamespaceParallelization.Extensions;

// We consider the Startup class in the namespace of the test cases to be the equivalent of a collection
// fixture. The constructor arguments can be any concrete type, which will be resolved recursively,
// and treated as top level startup instances (which are shared among all startup objects).
//
// Tests may only take ctor dependencies on the Startup object in their namespace, but not on any top
// level startup objects.

public class NamespaceParallelizationTestCollectionRunner :
    XunitTestCollectionRunnerBase<NamespaceParallelizationTestCollectionRunnerContext, IXunitTestCollection, IXunitTestClass, IXunitTestCase>
{
    // Used to ensure type creation is serialized
    static readonly SemaphoreSlim creationSemaphore = new(initialCount: 1);

    public static NamespaceParallelizationTestCollectionRunner Instance { get; } = new();

    protected override async ValueTask<bool> OnTestCollectionStarting(NamespaceParallelizationTestCollectionRunnerContext ctxt)
    {
        var @namespace = ctxt.TestCollection.TestCollectionDisplayName;
        var startupType = Type.GetType(@namespace.Length > 0 ? $"{@namespace}.Setup" : "Setup");
        if (startupType is not null)
        {
            var startup = await TryCreateType(ctxt, startupType);
            if (startup is not null)
                ctxt.StartupObject = startup;
        }

        return await base.OnTestCollectionStarting(ctxt);
    }

    public async ValueTask<RunSummary> Run(
        Dictionary<Type, object> topLevelSetupInstances,
        IXunitTestCollection testCollection,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var ctxt = new NamespaceParallelizationTestCollectionRunnerContext(topLevelSetupInstances, testCollection, testCases, explicitOption, messageBus, aggregator, cancellationTokenSource);
        await ctxt.InitializeAsync();

        return await Run(ctxt);
    }

    protected override ValueTask<RunSummary> RunTestClass(
        NamespaceParallelizationTestCollectionRunnerContext ctxt,
        IXunitTestClass? testClass,
        IReadOnlyCollection<IXunitTestCase> testCases)
    {
        if (testClass is null)
            return new(XunitRunnerHelper.FailTestCases(ctxt.MessageBus, ctxt.CancellationTokenSource, testCases, "Test case '{0}' must be backed by a test class"));

        return NamespaceParallelizationTestClassRunner.Instance.Run(ctxt.StartupObject, testClass, testCases, ctxt.ExplicitOption, ctxt.MessageBus, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource);
    }

    // Run everything in parallel
    protected override async ValueTask<RunSummary> RunTestClasses(
        NamespaceParallelizationTestCollectionRunnerContext ctxt,
        Exception? exception)
    {
        var collectionSummary = new RunSummary();
        var tasks = new List<Task<RunSummary>>();

        foreach (var testCasesByClass in ctxt.TestCases.GroupBy(tc => tc.TestMethod.TestClass, TestClassComparer.Instance))
        {
            var testClass = testCasesByClass.Key as IXunitTestClass;
            var testCases = testCasesByClass.ToArray();

            if (exception is not null)
                tasks.Add(Task.Run(async () => await FailTestClass(ctxt, testClass, testCases, exception), ctxt.CancellationTokenSource.Token));
            else
                tasks.Add(Task.Run(async () => await RunTestClass(ctxt, testClass, testCases), ctxt.CancellationTokenSource.Token));
        }

        var classSummaries = await Task.WhenAll(tasks);

        foreach (var classSummary in classSummaries)
            collectionSummary.Aggregate(classSummary);

        return collectionSummary;
    }

    // This will recursively try to resolve types that are in the constructor, on the assumption that
    // any value in the constructor is actually a top level startup class. Eventually we have to land
    // with a type that has a parameterless constructor.
    static async ValueTask<object?> TryCreateType(
        NamespaceParallelizationTestCollectionRunnerContext ctxt,
        Type type,
        bool alreadyLocked = false)
    {
        if (type.IsInterface || type.IsAbstract)
        {
            ctxt.Aggregator.Add(new TestPipelineException($"Startup type '{type.FullName}' is not a concrete type"));
            return null;
        }

        var ctors =
            type.GetTypeInfo()
                .DeclaredConstructors
                .Where(ci => !ci.IsStatic && ci.IsPublic)
                .ToList();

        if (ctors.Count != 1)
        {
            ctxt.Aggregator.Add(new TestPipelineException($"Startup type '{type.FullName}' may only define a single public constructor."));
            return null;
        }

        if (!alreadyLocked)
            await creationSemaphore.WaitAsync();

        try
        {
            var ctor = ctors[0];
            var ctorParams = ctor.GetParameters();
            var ctorArgs = new object?[ctorParams.Length];

            for (int i = 0; i < ctorParams.Length; ++i)
            {
                var paramType = ctorParams[i].ParameterType;
                if (!ctxt.TopLevelSetupInstances.TryGetValue(paramType, out var arg))
                {
                    arg = await TryCreateType(ctxt, paramType, alreadyLocked: true);
                    if (arg is null)
                        return null;

                    ctxt.TopLevelSetupInstances[paramType] = arg;
                }

                ctorArgs[i] = arg;
            }

            try
            {
                var result = ctor.Invoke(ctorArgs);
                if (result is null)
                    return null;

                if (result is IAsyncLifetime asyncLifetime)
                    await asyncLifetime.InitializeAsync();

                return result;
            }
            catch (Exception ex)
            {
                ctxt.Aggregator.Add(new TestPipelineException($"Exception while creating startup type '{type.FullName}': {ex}"));
                return null;
            }
        }
        finally
        {
            if (!alreadyLocked)
                creationSemaphore.Release();
        }
    }
}

public class NamespaceParallelizationTestCollectionRunnerContext(
    Dictionary<Type, object> topLevelSetupInstances,
    IXunitTestCollection testCollection,
    IReadOnlyCollection<IXunitTestCase> testCases,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource) :
        XunitTestCollectionRunnerBaseContext<IXunitTestCollection, IXunitTestCase>(testCollection, testCases, explicitOption, messageBus, DefaultTestCaseOrderer.Instance, aggregator, cancellationTokenSource, new("Unused"))
{
    public object? StartupObject { get; set; }

    public Dictionary<Type, object> TopLevelSetupInstances { get; } = topLevelSetupInstances;
}
