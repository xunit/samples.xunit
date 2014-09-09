using System;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationDiscoverer : TestFrameworkDiscoverer
    {
        readonly CollectionPerClassTestCollectionFactory testCollectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservationDiscoverer"/> class.
        /// </summary>
        /// <param name="assemblyInfo">The test assembly.</param>
        /// <param name="sourceProvider">The source information provider.</param>
        public ObservationDiscoverer(IAssemblyInfo assemblyInfo, ISourceInformationProvider sourceProvider)
            : base(assemblyInfo, sourceProvider, null)
        {
            var testAssembly = new TestAssembly(assemblyInfo, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            testCollectionFactory = new CollectionPerClassTestCollectionFactory(testAssembly);
        }

        protected override ITestClass CreateTestClass(ITypeInfo @class)
        {
            return new TestClass(testCollectionFactory.Get(@class), @class);
        }

        bool FindTestsForMethod(ITestMethod testMethod, bool includeSourceInformation, IMessageBus messageBus)
        {
            var observationAttribute = testMethod.Method.GetCustomAttributes(typeof(ObservationAttribute)).FirstOrDefault();
            if (observationAttribute == null)
                return true;

            var testCase = new ObservationTestCase(testMethod);
            if (!ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus))
                return false;

            return true;
        }

        protected override bool FindTestsForType(ITestClass testClass, bool includeSourceInformation, IMessageBus messageBus)
        {
            foreach (var method in testClass.Class.GetMethods(includePrivateMethods: true))
            {
                var testMethod = new TestMethod(testClass, method);
                if (!FindTestsForMethod(testMethod, includeSourceInformation, messageBus))
                    return false;
            }

            return true;
        }
    }
}
