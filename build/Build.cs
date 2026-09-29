using System;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Publish);

    AbsolutePath ProjectDirectory => RootDirectory / "geometry3Sharp";
    AbsolutePath ProjectFile => ProjectDirectory / "geometry3Sharp_netstandard.csproj";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    [Parameter("Configuration to build")]
    readonly Configuration Configuration = Configuration.Release;

    [Parameter("NuGet package version (for example, the release tag)")]
    readonly string PackageVersion = null;

    Target Clean => _ => _
        .Executes(() =>
        {
            ProjectDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() => DotNetRestore(s => s.SetProjectFile(ProjectFile)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => DotNetBuild(s => s
            .SetProjectFile(ProjectFile)
            .SetConfiguration(Configuration)
            .SetVersion(PackageVersion)
            .EnableNoRestore()));

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetPack(s => s
            .SetProject(ProjectFile)
            .SetConfiguration(Configuration)
            .SetVersion(PackageVersion)
            .SetOutputDirectory(ArtifactsDirectory)
            .EnableNoBuild()
            .EnableNoRestore()));

    Target Publish => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            var apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("NUGET_API_KEY is required to publish packages.");

            NuGetTasks.NuGetPush((options) =>
            options
                .SetApiKey(apiKey)
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetTargetPath(ArtifactsDirectory / "*.nupkg")
            );
        });
}