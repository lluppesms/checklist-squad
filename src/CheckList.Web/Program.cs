using CheckList.Web.Components;
using CheckList.Web.Data;
using CheckList.Web.Data.Repositories;
using CheckList.Web.Hubs;
using CheckList.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Entra ID (Azure AD) authentication via Microsoft.Identity.Web
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");

// Authorization: define "admin" and "user" policy roles
builder.Services.AddAuthorization(options =>
{
    // Users must be authenticated to access protected routes
    options.FallbackPolicy = null; // Don't force auth globally — individual pages use [Authorize]
});

// EF Core
builder.Services.AddDbContext<CheckListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ICheckRepository, CheckRepository>();

// Blazor (requires Razor Pages for OIDC callbacks)
builder.Services.AddRazorPages();
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

// Apply any pending schema changes to the database (idempotent – safe to run on every startup).
// This ensures the auth-related columns and tables exist even on databases created before
// the authentication feature was added.
await DatabaseSchemaService.ApplySchemaUpdatesAsync(
    app.Services,
    app.Logger);

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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Login/logout endpoints (redirect-based, compatible with Blazor Server)
app.MapGet("/account/login", (string? returnUrl) =>
{
    var props = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
    {
        RedirectUri = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl
    };
    return Results.Challenge(props, [OpenIdConnectDefaults.AuthenticationScheme]);
});

app.MapPost("/account/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/");
}).RequireAuthorization();

app.MapStaticAssets();
app.MapRazorPages();
app.MapControllers();
app.MapHub<CheckListHub>("/hubs/checklist");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
