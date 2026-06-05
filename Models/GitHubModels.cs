using System.Text.Json.Serialization;

namespace PortfolioForge.Models;

public class GitHubUser
{
    [JsonPropertyName("login")] public string Login { get; set; } = "";
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("bio")] public string? Bio { get; set; }
    [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
    [JsonPropertyName("html_url")] public string? HtmlUrl { get; set; }
    [JsonPropertyName("location")] public string? Location { get; set; }
    [JsonPropertyName("company")] public string? Company { get; set; }
    [JsonPropertyName("blog")] public string? Blog { get; set; }
    [JsonPropertyName("twitter_username")] public string? TwitterUsername { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("followers")] public int Followers { get; set; }
    [JsonPropertyName("public_repos")] public int PublicRepos { get; set; }
}

public class GitHubRepo
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("full_name")] public string FullName { get; set; } = "";
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = "";
    [JsonPropertyName("homepage")] public string? Homepage { get; set; }
    [JsonPropertyName("language")] public string? Language { get; set; }
    [JsonPropertyName("stargazers_count")] public int Stars { get; set; }
    [JsonPropertyName("forks_count")] public int Forks { get; set; }
    [JsonPropertyName("topics")] public List<string>? Topics { get; set; }
    [JsonPropertyName("fork")] public bool IsFork { get; set; }
    [JsonPropertyName("archived")] public bool IsArchived { get; set; }
    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("pushed_at")] public DateTime PushedAt { get; set; }
}
