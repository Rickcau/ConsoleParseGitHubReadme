
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Repo.Models;
public class RepoAnalysis
{
    [JsonPropertyName("description")]
    public string? Description { get; set; } = string.Empty;

    [JsonPropertyName("intendedUse")]
    public string? IntendedUse { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; } = new List<string>();

    [JsonPropertyName("author")]
    public string? Author { get; set; } = "Unknown";

    [JsonPropertyName("url")]
    public string? Url { get; set; } = "Unknown";
}