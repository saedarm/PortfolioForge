using System.Net.Http.Headers;
using System.Net.Http.Json;
using PortfolioForge.Models;

namespace PortfolioForge.Services;

public class GitHubService
{
    private readonly HttpClient _http;
    private string? _token;

    public GitHubService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>Set an optional GitHub personal access token to raise rate limits from 60/hr to 5000/hr.</summary>
    public void SetToken(string? token)
    {
        _token = string.IsNullOrWhiteSpace(token) ? null : token.Trim();
    }

    public bool HasToken => !string.IsNullOrEmpty(_token);

    private HttpRequestMessage BuildRequest(string path)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, path);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        if (!string.IsNullOrEmpty(_token))
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("token", _token);
        }
        return req;
    }

    public async Task<GitHubUser?> GetUserAsync(string username)
    {
        try
        {
            using var req = BuildRequest($"users/{Uri.EscapeDataString(username)}");
            using var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<GitHubUser>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<List<GitHubRepo>> GetReposAsync(string username, bool includeForks = false)
    {
        var all = new List<GitHubRepo>();
        var page = 1;
        while (true)
        {
            using var req = BuildRequest($"users/{Uri.EscapeDataString(username)}/repos?per_page=100&sort=updated&page={page}");
            using var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode) break;
            var batch = await res.Content.ReadFromJsonAsync<List<GitHubRepo>>();
            if (batch is null || batch.Count == 0) break;
            all.AddRange(batch);
            if (batch.Count < 100) break;
            page++;
            if (page > 5) break; // cap at 500 repos
        }
        return all
            .Where(r => includeForks || !r.IsFork)
            .Where(r => !r.IsArchived)
            .ToList();
    }
}
