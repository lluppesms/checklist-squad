var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapControllers();
app.MapHub<CheckList.Api.Hubs.CheckListHub>("/hubs/checklist");

app.Run();
