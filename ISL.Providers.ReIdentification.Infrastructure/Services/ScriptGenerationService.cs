﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV3s;

namespace ISL.Providers.ReIdentification.Infrastructure.Services
{
    internal class ScriptGenerationService
    {
        private readonly ADotNetClient adotNetClient;

        public ScriptGenerationService() =>
            adotNetClient = new ADotNetClient();

        public void GenerateBuildScript(string branchName, string projectName, string dotNetVersion)
        {
            var githubPipeline = new GithubPipeline
            {
                Name = ".Net",

                OnEvents = new Events
                {
                    Push = new PushEvent { Branches = [branchName] },

                    PullRequest = new PullRequestEvent
                    {
                        Types = ["opened", "synchronize", "reopened", "closed"],
                        Branches = [branchName]
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            Name = "Build",
                            RunsOn = BuildMachines.WindowsLatest,

                            EnvironmentVariables = new Dictionary<string, string>
                            {
                                { "REIDENTIFICATIONCONFIGURATIONS__APIURL", "${{ secrets.REIDENTIFICATIONCONFIGURATIONS__APIURL }}" },
                                { "REIDENTIFICATIONCONFIGURATIONS__APIKEY", "${{ secrets.REIDENTIFICATIONCONFIGURATIONS__APIKEY }}" },
                                { "REIDENTIFICATIONCONFIGURATIONS__DATAFILE", "${{ secrets.REIDENTIFICATIONCONFIGURATIONS__DATAFILE }}" },
                            },

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV3
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV3
                                {
                                    Name = "Setup .Net",

                                    With = new TargetDotNetVersionV3
                                    {
                                        DotNetVersion = "9.0.100"
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Build"
                                },

                                new TestTask
                                {
                                    Name = "Abstraction - Unit Tests",
                                    Run = "dotnet test ISL.Providers.ReIdentification.Abstractions.Tests.Unit/ISL.Providers.ReIdentification.Abstractions.Tests.Unit.csproj --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "Offline Provider - Unit Tests",
                                    Run = "dotnet test ISL.Providers.ReIdentification.OfflineFileSources.Tests.Unit/ISL.Providers.ReIdentification.OfflineFileSources.Tests.Unit.csproj --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "NECS Provider - Unit Tests",
                                    Run = "dotnet test ISL.Providers.ReIdentification.Necs.Tests.Unit/ISL.Providers.ReIdentification.Necs.Tests.Unit.csproj --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "Abstraction - Acceptance Tests",
                                    Run = "dotnet test ISL.Providers.ReIdentification.Abstractions.Tests.Acceptance/ISL.Providers.ReIdentification.Abstractions.Tests.Acceptance.csproj --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "Offline Provider - Acceptance Tests",
                                    Run = "dotnet test ISL.Providers.ReIdentification.OfflineFileSources.Tests.Acceptance/ISL.Providers.ReIdentification.OfflineFileSources.Tests.Acceptance.csproj --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "NECS Provider - Acceptance Tests",
                                    Run = "dotnet test ISL.Providers.ReIdentification.Necs.Tests.Acceptance/ISL.Providers.ReIdentification.Necs.Tests.Acceptance.csproj --no-build --verbosity normal"
                                },
                            }
                        }
                    },
                    {
                        "add_tag",
                        new TagJob(
                            runsOn: BuildMachines.UbuntuLatest,
                            dependsOn: "build",
                            projectRelativePath: $"{projectName}/{projectName}.csproj",
                            githubToken: "${{ secrets.PAT_FOR_TAGGING }}",
                            branchName: branchName)
                        {
                            Name = "Tag and Release"
                        }
                    },
                    {
                        "publish",
                        new PublishJobV2(
                            runsOn: BuildMachines.UbuntuLatest,
                            dependsOn: "add_tag",
                            dotNetVersion: dotNetVersion,
                            nugetApiKey: "${{ secrets.NUGET_ACCESS }}")
                        {
                            Name = "Publish to NuGet"
                        }
                    }
                }
            };

            string buildScriptPath = "../../../../.github/workflows/build.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }

        public void GeneratePrLintScript(string branchName)
        {
            var githubPipeline = new GithubPipeline
            {
                Name = "PR Linter",

                OnEvents = new Events
                {
                    PullRequest = new PullRequestEvent
                    {
                        Types = ["opened", "edited", "synchronize", "reopened", "closed"],
                        Branches = [branchName]
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "label",
                        new LabelJobV2(runsOn: BuildMachines.UbuntuLatest)
                        {
                            Name = "Add Label(s)",
                        }
                    },
                    {
                        "requireIssueOrTask",
                        new RequireIssueOrTaskJob()
                        {
                            Name = "Require Issue Or Task Association",
                        }
                    },
                }
            };

            string buildScriptPath = "../../../../.github/workflows/prLinter.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }
    }
}
