namespace CheckList.Web.Services;

/// <summary>
/// Reads build metadata from wwwroot/buildinfo.json.
/// </summary>
public interface IBuildInfoService
{
    Task<BuildInfo?> GetBuildInfoAsync();
}
