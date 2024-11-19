using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace ObservationExample;

public class ObservationDiscoverer(ObservationTestAssembly testAssembly) :
    TestFrameworkDiscoverer<ObservationTestClass>(testAssembly)
{
    public new ObservationTestAssembly TestAssembly { get; } = testAssembly;

    protected override ValueTask<ObservationTestClass> CreateTestClass(Type @class) =>
        new(new ObservationTestClass(TestAssembly, @class));

    static async ValueTask<bool> FindTestsForMethod(
        ObservationTestMethod testMethod,
        ITestFrameworkDiscoveryOptions discoveryOptions,
        Func<ObservationTestCase, ValueTask<bool>> discoveryCallback)
    {
        var observationAttribute = testMethod.Method.GetCustomAttributes<ObservationAttribute>().FirstOrDefault();
        if (observationAttribute is null)
            return true;

        var order = observationAttribute.Order;

        var testCase = new ObservationTestCase(testMethod, order);
        if (!await discoveryCallback(testCase))
            return false;

        return true;
    }

    protected override async ValueTask<bool> FindTestsForType(
        ObservationTestClass testClass,
        ITestFrameworkDiscoveryOptions discoveryOptions,
        Func<ITestCase, ValueTask<bool>> discoveryCallback)
    {
        if (!typeof(Specification).IsAssignableFrom(testClass.Class))
            return true;

        foreach (var method in testClass.Methods)
        {
            var testMethod = new ObservationTestMethod(testClass, method);

            try
            {
                if (!await FindTestsForMethod(testMethod, discoveryOptions, discoveryCallback))
                    return false;
            }
            catch (Exception ex)
            {
                TestContext.Current.SendDiagnosticMessage("Exception during discovery of test class {0}:{1}{2}", testClass.Class.FullName, Environment.NewLine, ex);
            }
        }

        return true;
    }

    protected override Type[] GetExportedTypes() =>
        TestAssembly.Assembly.ExportedTypes.ToArray();
}
