using CheckList.Web.Components;
using CheckList.Web.Data;
using CheckList.Web.Data.Repositories;
using CheckList.Web.Hubs;
using CheckList.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// EF Core
builder.Services.AddDbContext<CheckListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ICheckRepository, CheckRepository>();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Controllers + SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Swagger for dev
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application services
builder.Services.AddScoped<ICheckListApiClient, CheckListService>();
builder.Services.AddScoped<IUserIdentity, UserIdentityService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapHub<CheckListHub>("/hubs/checklist");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
