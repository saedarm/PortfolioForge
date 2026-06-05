using System.Net;
using System.Text;
using PortfolioForge.Models;

namespace PortfolioForge.Services.Templates;

/// <summary>Shared helpers for generating consistent social link HTML across templates.</summary>
internal static class SocialLinks
{
    public record SocialLink(string Label, string Url, string Icon);

    public static List<SocialLink> BuildList(GitHubUser user)
    {
        var links = new List<SocialLink>
        {
            new("GitHub", user.HtmlUrl ?? $"https://github.com/{user.Login}", "github")
        };
        if (!string.IsNullOrWhiteSpace(user.TwitterUsername))
            links.Add(new("Twitter", $"https://twitter.com/{user.TwitterUsername}", "twitter"));
        if (!string.IsNullOrWhiteSpace(user.Blog))
        {
            var blog = user.Blog!.StartsWith("http") ? user.Blog : $"https://{user.Blog}";
            links.Add(new("Site", blog, "globe"));
        }
        if (!string.IsNullOrWhiteSpace(user.Email))
            links.Add(new("Email", $"mailto:{user.Email}", "mail"));
        return links;
    }

    /// <summary>Render social links as inline-styled anchors. Caller styles via .social-links class.</summary>
    public static string RenderHtml(GitHubUser user, string cssClass = "social-links")
    {
        var links = BuildList(user);
        var sb = new StringBuilder();
        sb.Append($"<div class=\"{cssClass}\">");
        foreach (var l in links)
        {
            sb.Append($"<a href=\"{Esc(l.Url)}\" target=\"_blank\" rel=\"noopener\" data-icon=\"{l.Icon}\">{l.Label}</a>");
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string Esc(string? s) => WebUtility.HtmlEncode(s ?? "");
}
