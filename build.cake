///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solution = "./WeihanLi.Redis.sln";
var srcProjects = "./src/**/*.csproj";
var testProjects = "./test/**/*.csproj";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

// TaskSetup(setupContext =>
// {
//     var message = string.Format("Task: {0}", setupContext.Task.Name);
//     // custom logging
// });

// TaskTeardown(teardownContext =>
// {
//     var message = string.Format("Task: {0}", teardownContext.Task.Name);
//     // custom logging
// });

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("clean")
    .Description("Clean")
    .Does(() =>
    {
      if (DirectoryExists("./artifacts"))
      {
         DeleteDirectory("./artifacts", true);
      }
    });

Task("restore")
    .Description("Restore")
    .Does(() => 
    {
       DotNetCoreRestore(solutionPath);
    });

Task("build")    
    .Description("Build")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
    {
      DotNetCoreRestore(solutionPath);
    });

Task("test")    
    .Description("Build")
    .IsDependentOn("build")
    .Does(() =>
    {
        foreach (var project in testProjects)
        {
            DotNetCoreTest(project.FullPath);
        }
    });


Task("pack")
    .Description("Pack package")
    .IsDependentOn("test")
    .Does(() =>
    {
      var settings = new DotNetCorePackSettings
      {
         Configuration = configuration,
         OutputDirectory = "./artifacts/packages"
      };
      foreach (var project in srcProjects)
      {
         DotNetCorePack(project.FullPath, settings);
      }
    });

Task("Default")
    .IsDependentOn("test");

RunTarget(target);