// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Infrastructure.Services;

namespace ISL.Providers.ReIdentification.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "ISL.Providers.ReIdentification.Abstractions");
        }
    }
}
