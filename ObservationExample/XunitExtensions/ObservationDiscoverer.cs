﻿using System;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationDiscoverer : TestFrameworkDiscoverer
    {
        readonly CollectionPerClassTestCollectionFactory testCollectionFactory;

        public ObservationDiscoverer(IAssemblyInfo assemblyInfo,
                                     ISourceInformationProvider sourceProvider,
                                     IMessageSink diagnosticMessageSink)
            : base(assemblyInfo, sourceProvider, diagnosticMessageSink)
        {
            var testAssembly = new TestAssembly(assemblyInfo, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            testCollectionFactory = new CollectionPerClassTestCollectionFactory(testAssembly, diagnosticMessageSink);
        }

        protected override ITestClass CreateTestClass(ITypeInfo @class)
        {
            return new TestClass(testCollectionFactory.Get(@class), @class);
        }

        bool FindTestsForMethod(ITestMethod testMethod,
                                TestMethodDisplay defaultMethodDisplay,
                                TestMethodDisplayOptions defaultMethodDisplayOptions,
                                bool includeSourceInformation,
                                IMessageBus messageBus)
        {
            var observationAttribute = testMethod.Method.GetCustomAttributes(typeof(ObservationAttribute)).FirstOrDefault();
            if (observationAttribute == null)
                return true;

            var order = observationAttribute.GetNamedArgument<int>("Order");

            var testCase = new ObservationTestCase(defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, order);
            if (!ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus))
                return false;

            return true;
        }

        protected override bool FindTestsForType(ITestClass testClass,
                                                 bool includeSourceInformation,
                                                 IMessageBus messageBus,
                                                 ITestFrameworkDiscoveryOptions discoveryOptions)
        {
            var methodDisplay = discoveryOptions.MethodDisplayOrDefault();
            var methodDisplayOptions = discoveryOptions.MethodDisplayOptionsOrDefault();

            foreach (var method in testClass.Class.GetMethods(includePrivateMethods: true))
                if (!FindTestsForMethod(new TestMethod(testClass, method), methodDisplay, methodDisplayOptions, includeSourceInformation, messageBus))
                    return false;

            return true;
        }
    }
}
