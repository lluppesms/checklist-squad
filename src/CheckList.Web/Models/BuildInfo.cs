using System.Diagnostics.CodeAnalysis;

namespace CheckList.Web.Models;

/// <summary>
/// Build information populated from buildinfo.json at deploy time.
/// </summary>
[ExcludeFromCodeCoverage]
public class BuildInfo
{
    [JsonPropertyName("buildDate")]
    public string BuildDate { get; set; } = string.Empty;

    [JsonPropertyName("buildNumber")]
    public string BuildNumber { get; set; } = string.Empty;

    [JsonPropertyName("buildId")]
    public string BuildId { get; set; } = string.Empty;

    [JsonPropertyName("branchName")]
    public string BranchName { get; set; } = string.Empty;

    [JsonPropertyName("commitHash")]
    public string CommitHash { get; set; } = string.Empty;
}
