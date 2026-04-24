namespace CheckList.Web.Services;

/// <summary>
/// Reads and caches build metadata from wwwroot/buildinfo.json.
/// </summary>
public class BuildInfoService : IBuildInfoService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<BuildInfoService> _logger;
    private BuildInfo? _cached;

    public BuildInfoService(IWebHostEnvironment env, ILogger<BuildInfoService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<BuildInfo?> GetBuildInfoAsync()
    {
        if (_cached is not null)
        {
            return _cached;
        }

        try
        {
            var filePath = Path.Combine(_env.WebRootPath, "buildinfo.json");
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                _cached = JsonSerializer.Deserialize<BuildInfo>(json);
                return _cached;
            }

            _logger.LogInformation("buildinfo.json not found at {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load build info");
        }

        return null;
    }
}
