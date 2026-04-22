var builder = DistributedApplication.CreateBuilder(args);

var web = builder.AddProject<Projects.CheckList_Web>("checklist-web");

builder.Build().Run();
