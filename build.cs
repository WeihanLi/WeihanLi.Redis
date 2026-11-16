var solutionPath = "./WeihanLi.Redis.slnx";
string[] srcProjects = [ 
    "./src/WeihanLi.Redis/WeihanLi.Redis.csproj"
];
string[] testProjects = [ "./test/WeihanLi.Redis.UnitTest/WeihanLi.Redis.UnitTest.csproj" ];

await DotNetPackageBuildProcess
    .Create(options => 
    {
        options.SolutionPath = solutionPath;
        options.SrcProjects = srcProjects;
        options.TestProjects = EnvHelper.Val("BUILD_SOURCEBRANCHNAME") is not null ? [] : testProjects;
    })
    .ExecuteAsync(args);
