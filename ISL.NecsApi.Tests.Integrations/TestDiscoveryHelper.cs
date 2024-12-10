// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.NecsApi.Tests.Integrations
{
    //public static class TestDiscoveryHelper
    //{
    //    public static IEnumerable<IXunitTestCase> DiscoverTheoryTestCases(
    //        IMessageSink diagnosticMessageSink,
    //        ITestFrameworkDiscoveryOptions discoveryOptions,
    //        ITestMethod testMethod)
    //    {
    //        // Use Xunit's built-in discoverer to handle theory cases
    //        var dataAttributes = testMethod.Method
    //            .GetCustomAttributes(typeof(DataAttribute))
    //            .ToList();

    //        if (dataAttributes.Count == 0)
    //        {
    //            yield return new ExecutionErrorTestCase(
    //                diagnosticMessageSink,
    //                TestMethodDisplay.Method,
    //                testMethod,
    //                $"Theory method '{testMethod.Method.Name}' on '{testMethod.TestClass.Class.Name}' has no data attributes.");
    //            yield break;
    //        }

    //        foreach (var dataAttribute in dataAttributes)
    //        {
    //            var dataDiscoverer = ExtensibilityPointFactory.GetDataDiscoverer(dataAttribute);

    //            if (dataDiscoverer == null)
    //            {
    //                yield return new ExecutionErrorTestCase(
    //                    diagnosticMessageSink,
    //                    TestMethodDisplay.Method,
    //                    testMethod,
    //                    $"Unable to find data discoverer for data attribute '{dataAttribute.GetType().FullName}' on '{testMethod.Method.Name}'.");
    //                continue;
    //            }

    //            foreach (var testCase in dataDiscoverer.GetData(dataAttribute, testMethod))
    //            {
    //                yield return new XunitTheoryTestCase(
    //                    diagnosticMessageSink,
    //                    discoveryOptions.MethodDisplayOrDefault(),
    //                    discoveryOptions.MethodDisplayOptionsOrDefault(),
    //                    testMethod,
    //                    testCase);
    //            }
    //        }
    //    }
    //}

}
