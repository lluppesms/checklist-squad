using CheckList.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// EF Core
builder.Services.AddDbContext<CheckListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ICheckRepository, CheckRepository>();

// Controllers + SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Swagger for dev
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.MapControllers();
app.MapHub<CheckListHub>("/hubs/checklist");

app.Run();
