// Install .NET Core Global tools.
#tool "dotnet:?package=dotnet-reportgenerator-globaltool&version=5.4.9"
#tool "dotnet:?package=coveralls.net&version=4.0.1"
#tool "dotnet:?package=dotnet-sonarscanner&version=11.0.0"

// Install addins
#addin nuget:?package=Cake.Coverlet&version=5.1.1
#addin nuget:?package=Cake.Sonar&version=5.0.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifactsPath = "./artifacts";
var coveragePath = "./artifacts/coverage";
var packFiles = "./src/**/*.csproj";
var testFiles = "./test/**/*.csproj";
var packages = "./artifacts/*.nupkg";

uint coverageThreshold = 50;

var sonarToken = EnvironmentVariable("SONAR_TOKEN");
var sonarStarted = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
   BuildContext.Initialize(Context);
   Information($"Building TrueLayer.NET with configuration {configuration}");
});

Teardown(ctx =>
{
   if (DirectoryExists(coveragePath))
   {
        DeleteDirectory(coveragePath, new DeleteDirectorySettings
        {
            Recursive = true,
            Force = true
        });
   }

   Information("Finished running build");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(artifactsPath);
    });

Task("SonarBegin")
    .WithCriteria(!string.IsNullOrEmpty(sonarToken))
    .ContinueOnError()
    .Does(() =>
    {
        try
        {
            Information("Starting SonarCloud analysis...");
            SonarBegin(new SonarBeginSettings
            {
                Key = "TrueLayer_truelayer-dotnet",
                Organization = "truelayer",
                Url = "https://sonarcloud.io",
                Exclusions = "test/**,examples/**,**/MvcExample/**,artifacts/**,docs/**,**/*.png,**/*.jpg,**/*.jpeg,**/*.gif,**/*.svg,**/*.ico,**/*.pem",
                OpenCoverReportsPath = $"{coveragePath}/*.xml",
                Token = sonarToken,
                VsTestReportsPath = $"{artifactsPath}/*.TestResults.xml",
                ArgumentCustomization = args => args
                    .Append("/d:sonar.scm.disabled=true")
                    .Append("/d:sonar.scanner.skipJreProvisioning=true")
            });

            // Verify the config file was created
            var configFile = ".sonarqube/conf/SonarQubeAnalysisConfig.xml";
            if (FileExists(configFile))
            {
                sonarStarted = true;
                Information("SonarCloud analysis started successfully - config file created");
            }
            else
            {
                sonarStarted = false;
                Warning("SonarCloud analysis may not have started correctly - config file not found");
            }
        }
        catch (Exception ex)
        {
            sonarStarted = false;
            Warning($"SonarCloud analysis start failed (non-blocking): {ex.Message}");
            if (ex.InnerException != null)
            {
                Warning($"Inner exception: {ex.InnerException.Message}");
            }
        }
    });

Task("Build")
    .Does(() =>
    {
        DotNetBuild("TrueLayer.sln", new DotNetBuildSettings
        {
            Configuration = configuration
        });
    });

Task("Test")
   .Does(() =>
   {
        foreach (var project in GetFiles(testFiles))
        {
            var projectName = project.GetFilenameWithoutExtension();

            var testSettings = new DotNetTestSettings
            {
                NoBuild = true,
                Configuration = configuration,
                Loggers = { $"trx;LogFileName={projectName}.TestResults.xml" },
                ResultsDirectory = artifactsPath
            };

            // https://github.com/Romanx/Cake.Coverlet
            var coverletSettings = new CoverletSettings
            {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover,
                CoverletOutputDirectory = coveragePath,
                CoverletOutputName = $"{projectName}.opencover.xml"
                //Threshold = coverageThreshold
            };

            DotNetTest(project.ToString(), testSettings, coverletSettings);
        }
   });

Task("Pack")
    .Does(() =>
    {
        var settings = new DotNetPackSettings
        {
            Configuration = configuration,
            OutputDirectory = artifactsPath,
            NoBuild = true
        };

        foreach (var file in GetFiles(packFiles))
        {
            DotNetPack(file.ToString(), settings);
        }
    });

Task("GenerateReports")
    .Does(() =>
    {
        ReportGenerator(GetFiles($"{coveragePath}/*.xml"), artifactsPath, new ReportGeneratorSettings
        {
            ArgumentCustomization = args => args.Append("-reporttypes:lcov;HTMLSummary;TextSummary;")
        });
    });

Task("SonarEnd")
    .WithCriteria(() => sonarStarted)
    .ContinueOnError()
    .Does(() =>
    {
        try
        {
            SonarEnd(new SonarEndSettings
            {
                Token = sonarToken
            });
            Information("SonarCloud analysis completed successfully");
        }
        catch (Exception ex)
        {
            Warning($"SonarCloud analysis end failed (non-blocking): {ex.Message}");
        }
    });

Task("PublishPackages")
    .WithCriteria(() => BuildContext.ShouldPublishToNuget)
    .Does(() =>
    {
        foreach(var package in GetFiles(packages))
        {
            DotNetNuGetPush(package.ToString(), new DotNetNuGetPushSettings {
                ApiKey = BuildContext.NugetApiKey,
                Source = BuildContext.NugetApiUrl,
                SkipDuplicate = true
            });
        }
    });

Task("Dump").Does(() => BuildContext.PrintParameters(Context));

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack")
    .IsDependentOn("GenerateReports");

Task("CI")
    .IsDependentOn("SonarBegin")
    .IsDependentOn("Default")
    .IsDependentOn("SonarEnd");

Task("Publish")
    .IsDependentOn("CI")
    .IsDependentOn("PublishPackages");

RunTarget(target);

public static class BuildContext
{
    public static bool IsTag { get; private set; }
    public static string NugetApiUrl { get; private set; }
    public static string NugetApiKey { get; private set; }
    public static bool ForcePushDocs { get; private set; }

    public static bool ShouldPublishToNuget
        => !string.IsNullOrWhiteSpace(BuildContext.NugetApiUrl) && !string.IsNullOrWhiteSpace(BuildContext.NugetApiKey);

    public static void Initialize(ICakeContext context)
    {
        if (context.BuildSystem().IsRunningOnGitHubActions)
        {
            // https://github.com/cake-contrib/Cake.Recipe/blob/3ee5725b1cc0621f90205904848407515a2b62fd/Cake.Recipe/Content/github-actions.cake
            var tempName = context.BuildSystem().GitHubActions.Environment.Workflow.Ref;
            if (!string.IsNullOrEmpty(tempName) && tempName.IndexOf("tags/") >= 0)
            {
                IsTag = true;
                //Name = tempName.Substring(tempName.LastIndexOf('/') + 1);
            }
        }

        if (BuildContext.IsTag)
        {
            NugetApiUrl = context.EnvironmentVariable("NUGET_API_URL");
            NugetApiKey = context.EnvironmentVariable("NUGET_API_KEY");
        }
        else
        {
            NugetApiUrl = context.EnvironmentVariable("NUGET_PRE_API_URL");
            NugetApiKey = context.EnvironmentVariable("NUGET_PRE_API_KEY");
        }

        ForcePushDocs = context.Argument<bool>("force-docs", false);
    }

    public static void PrintParameters(ICakeContext context)
    {
        context.Information("Printing Build Parameters...");
        context.Information("IsTag: {0}", IsTag);
        context.Information("NugetApiUrl: {0}", NugetApiUrl);
        context.Information("NugetApiKey: {0}", NugetApiKey);
        context.Information("ShouldPublishToNuget: {0}", ShouldPublishToNuget);
    }
}
