// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.NecsApi.Tests.Integrations
{
    //public class ReleaseCandidateTheoryDiscoverer : IXunitTestCaseDiscoverer
    //{
    //    private readonly IMessageSink diagnosticMessageSink;
    //    private readonly IConfiguration configuration;

    //    public ReleaseCandidateTheoryDiscoverer(IMessageSink diagnosticMessageSink)
    //    {
    //        this.diagnosticMessageSink = diagnosticMessageSink;

    //        var configurationBuilder = new ConfigurationBuilder()
    //            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    //            .AddEnvironmentVariables();

    //        configuration = configurationBuilder.Build();
    //    }

    //    public IEnumerable<IXunitTestCase> Discover(
    //        ITestFrameworkDiscoveryOptions discoveryOptions,
    //        ITestMethod testMethod,
    //        IAttributeInfo factAttribute)
    //    {
    //        var isReleaseCandidate = configuration.GetValue<bool>("IS_RELEASE_CANDIDATE");

    //        if (isReleaseCandidate)
    //        {
    //            foreach (var testCase in TestDiscoveryHelper.DiscoverTheoryTestCases(
    //                diagnosticMessageSink, discoveryOptions, testMethod))
    //            {
    //                yield return testCase;
    //            }
    //        }
    //    }
    //}

}
