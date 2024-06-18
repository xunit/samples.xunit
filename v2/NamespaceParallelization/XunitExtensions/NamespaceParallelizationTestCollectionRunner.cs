using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NamespaceParallelization.XunitExtensions;

// We consider the Startup class in the namespace of the test cases to be the collection fixture. It may have
// constructor arguments of IMessageSink (this is normal for fixtures) or any creatable (concrete) type. Any
// constructor arguments of concrete types are resolved recursively, and assumed to be "top level startup
// instances" which means a single instance is shared amongst all startup objects. This means a startup
// object can take dependencies on a top level startup object, but not another startup object; the same
// is true for top level startup objects (though in reality we don't expect to see these nested dependencies).
// Tests may only take ctor dependencies on their Startup object in their namespace, not on any top level
// startup objects.

public class NamespaceParallelizationTestCollectionRunner(ITestCollection testCollection,
                                                          IEnumerable<IXunitTestCase> testCases,
                                                          IMessageSink diagnosticMessageSink,
                                                          IMessageBus messageBus,
                                                          ITestCaseOrderer testCaseOrderer,
                                                          ExceptionAggregator aggregator,
                                                          Dictionary<Type, object?> topLevelSetupInstances,
                                                          CancellationTokenSource cancellationTokenSource)
    : XunitTestCollectionRunner(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
{
    static readonly SemaphoreSlim creationSemaphore = new(initialCount: 1);

    // We'll look for a class in our namespace (which is the test collection name) named "Setup" and consider that to
    // be our collection fixture.
    protected override async Task AfterTestCollectionStartingAsync()
    {
        var @namespace = TestCollection.DisplayName;
        var startupType = Type.GetType(@namespace.Length > 0 ? $"{@namespace}.Setup" : "Setup");
        if (startupType is not null)
        {
            var startup = await TryCreateType(startupType);
            if (startup is not null)
                CollectionFixtureMappings[startupType] = startup;
        }
    }

    protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass,
                                                          IReflectionTypeInfo @class,
                                                          IEnumerable<IXunitTestCase> testCases)
        => new NamespaceParallelizationTestClassRunner(testClass, @class, testCases, DiagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings).RunAsync();

    // Override to run the classes in parallel
    protected override async Task<RunSummary> RunTestClassesAsync()
    {
        var collectionSummary = new RunSummary();
        var tasks = new List<Task<RunSummary>>();

        foreach (var testCasesByClass in TestCases.GroupBy(tc => tc.TestMethod.TestClass, TestClassComparer.Instance))
            tasks.Add(Task.Run(() => RunTestClassAsync(testCasesByClass.Key, (IReflectionTypeInfo)testCasesByClass.Key.Class, testCasesByClass), CancellationTokenSource.Token));

        var classSummaries = await Task.WhenAll(tasks);

        foreach (var classSummary in classSummaries)
            collectionSummary.Aggregate(classSummary);

        return collectionSummary;
    }

    // This will recursively try to resolve types that are in the constructor, on the assumption that
    // any value in the constructor is actually a top level startup class. Eventually we have to land
    // with a type that either has a parameterless constructor, or its only constructor argument is
    // an instance of IMessageSink.
    async Task<object?> TryCreateType(Type type, bool alreadyLocked = false)
    {
        if (type.IsInterface || type.IsAbstract)
        {
            Aggregator.Add(new TestClassException($"Startup type '{0}' is not a concrete type"));
            return null;
        }

        var ctors = type.GetTypeInfo()
                        .DeclaredConstructors
                        .Where(ci => !ci.IsStatic && ci.IsPublic)
                        .ToList();

        if (ctors.Count != 1)
        {
            Aggregator.Add(new TestClassException($"Startup type '{type.FullName}' may only define a single public constructor."));
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
                if (paramType == typeof(IMessageSink))
                    ctorArgs[i] = DiagnosticMessageSink;
                else
                {
                    if (!topLevelSetupInstances.TryGetValue(paramType, out var arg))
                    {
                        arg = await TryCreateType(paramType, alreadyLocked: true);
                        if (arg is null)
                            return null;

                        topLevelSetupInstances[paramType] = arg;
                    }

                    ctorArgs[i] = arg;
                }
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
                Aggregator.Add(new TestClassException($"Exception while creating startup type '{type.FullName}': {ex}"));
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
