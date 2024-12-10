// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xunit.Sdk;

namespace ISL.NecsApi.Tests.Integrations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer(
        typeName: "ISL.NecsApi.Tests.Integrations.ReleaseCandidateTestCaseDiscoverer",
        assemblyName: "ISL.NecsApi.Tests.Integrations")]
    public class ReleaseCandidateFactAttribute : FactAttribute { }
}
