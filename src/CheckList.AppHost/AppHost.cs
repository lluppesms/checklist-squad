var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.CheckList_Api>("checklist-api");
var web = builder.AddProject<Projects.CheckList_Web>("checklist-web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
