using System.Net;
using System.Text;
using PortfolioForge.Models;

namespace PortfolioForge.Services.Templates;

public class MinimalTemplate : IPortfolioTemplate
{
    public string Name => "Minimal";
    public string Description => "Paper white. Refined. Restraint as a feature.";
    public int VariantCount => 4;

    private record Variant(string Label, string Bg, string Ink, string Muted, string Accent, string Border, string MainFont, string MonoFont, string GoogleFontUrl);

    private static readonly Variant[] _variants =
    {
        new("Newsreader", "#fafaf7", "#1d1d1f", "#666",    "#b8860b", "#eae8e0",
            "'Newsreader', Georgia, serif",
            "'JetBrains Mono', monospace",
            "https://fonts.googleapis.com/css2?family=Newsreader:opsz,wght@6..72,300;6..72,400;6..72,600&family=JetBrains+Mono:wght@400&display=swap"),
        new("Bricolage",  "#ffffff", "#0d0d0d", "#666",    "#2f6df0", "#ececec",
            "'Bricolage Grotesque', sans-serif",
            "'JetBrains Mono', monospace",
            "https://fonts.googleapis.com/css2?family=Bricolage+Grotesque:opsz,wght@12..96,300;12..96,400;12..96,600;12..96,700&family=JetBrains+Mono&display=swap"),
        new("Fraunces",   "#f5efe6", "#1c1812", "#5e564a", "#b8410c", "#e0d8c8",
            "'Fraunces', Georgia, serif",
            "'JetBrains Mono', monospace",
            "https://fonts.googleapis.com/css2?family=Fraunces:opsz,wght@9..144,300;9..144,400;9..144,600&family=JetBrains+Mono&display=swap"),
        new("Sepia Lora", "#f0e9d8", "#2a1f10", "#6b5544", "#3b6e1b", "#d8cdb6",
            "'Lora', Georgia, serif",
            "'JetBrains Mono', monospace",
            "https://fonts.googleapis.com/css2?family=Lora:ital,wght@0,400;0,500;0,600;1,400&family=JetBrains+Mono&display=swap")
    };

    public GeneratedPortfolio Generate(GitHubUser user, List<GitHubRepo> repos, int variant)
    {
        var v = _variants[Math.Abs(variant) % _variants.Length];
        var displayName = WebUtility.HtmlEncode(user.Name ?? user.Login);
        var bio = WebUtility.HtmlEncode(user.Bio ?? "");

        var repoListHtml = new StringBuilder();
        foreach (var r in repos.OrderByDescending(r => r.Stars).Take(20))
        {
            var name = WebUtility.HtmlEncode(r.Name);
            var desc = WebUtility.HtmlEncode(r.Description ?? "");
            var lang = WebUtility.HtmlEncode(r.Language ?? "");
            repoListHtml.AppendLine($@"        <li class=""item"">
          <div class=""item-main"">
            <a class=""item-name"" href=""{r.HtmlUrl}"" target=""_blank"">{name}</a>
            <p class=""item-desc"">{desc}</p>
          </div>
          <div class=""item-meta"">
            <span>{lang}</span>
            <span class=""star"">★ {r.Stars}</span>
          </div>
        </li>");
        }

        var avatarHtml = !string.IsNullOrEmpty(user.AvatarUrl)
            ? $@"<img class=""avatar"" src=""{WebUtility.HtmlEncode(user.AvatarUrl)}"" alt=""{displayName}"" />"
            : "";

        var socialList = SocialLinks.BuildList(user);
        var linksHtml = new StringBuilder();
        foreach (var s in socialList)
        {
            linksHtml.Append($@"<a href=""{WebUtility.HtmlEncode(s.Url)}"" target=""_blank"">{WebUtility.HtmlEncode(s.Label.ToLower())}</a>");
        }

        var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{displayName}</title>
  <meta property=""og:title"" content=""{displayName}"">
  <meta property=""og:description"" content=""{bio}"">
  {(string.IsNullOrEmpty(user.AvatarUrl) ? "" : $@"<meta property=""og:image"" content=""{WebUtility.HtmlEncode(user.AvatarUrl)}"">")}
  <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
  <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
  <link href=""{v.GoogleFontUrl}"" rel=""stylesheet"">
  <link rel=""stylesheet"" href=""style.css"">
</head>
<body>
  <div class=""container"">
    <header>
      {avatarHtml}
      <h1>{displayName}</h1>
      {(string.IsNullOrEmpty(bio) ? "" : $@"<p class=""bio"">{bio}</p>")}
      <div class=""links"">
        {linksHtml}
        {(string.IsNullOrEmpty(user.Location) ? "" : $@"<span class=""loc"">{WebUtility.HtmlEncode(user.Location)}</span>")}
      </div>
    </header>
    <main>
      <h2>Projects <span class=""count"">/ {Math.Min(repos.Count, 20)}</span></h2>
      <ul class=""list"">
{repoListHtml}
      </ul>
    </main>
    <footer>
      <p>{user.PublicRepos} public repositories · {user.Followers} followers · {v.Label}</p>
    </footer>
  </div>
  <script src=""script.js""></script>
</body>
</html>";

        var css = $@"* {{ margin: 0; padding: 0; box-sizing: border-box; }}
body {{
  background: {v.Bg};
  color: {v.Ink};
  font-family: {v.MainFont};
  font-size: 18px;
  line-height: 1.55;
  -webkit-font-smoothing: antialiased;
}}
.container {{
  max-width: 640px;
  margin: 0 auto;
  padding: 6rem 2rem 4rem;
}}
header {{ margin-bottom: 6rem; }}
.avatar {{
  width: 72px;
  height: 72px;
  border-radius: 50%;
  object-fit: cover;
  margin-bottom: 2rem;
  display: block;
}}
h1 {{
  font-family: {v.MainFont};
  font-size: clamp(2.5rem, 6vw, 4rem);
  font-weight: 300;
  letter-spacing: -1.5px;
  line-height: 1.05;
  margin-bottom: 1rem;
}}
.bio {{
  font-size: 1.2rem;
  color: {v.Muted};
  font-weight: 300;
  max-width: 480px;
  margin-bottom: 2rem;
}}
.links {{
  display: flex;
  gap: 1.5rem;
  font-family: {v.MonoFont};
  font-size: 0.85rem;
  align-items: center;
  flex-wrap: wrap;
}}
.links a {{
  color: {v.Ink};
  text-decoration: none;
  border-bottom: 1px solid #ccc;
  padding-bottom: 1px;
  transition: border-color 0.2s, color 0.2s;
}}
.links a:hover {{ color: {v.Accent}; border-color: {v.Accent}; }}
.loc {{ color: {v.Muted}; }}
h2 {{
  font-family: {v.MainFont};
  font-size: 1.5rem;
  font-weight: 400;
  margin-bottom: 2.5rem;
  color: {v.Ink};
}}
.count {{
  font-family: {v.MonoFont};
  font-size: 0.85rem;
  color: #999;
  font-weight: 400;
  margin-left: 0.5rem;
}}
.list {{ list-style: none; }}
.item {{
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 2rem;
  padding: 1.5rem 0;
  border-bottom: 1px solid {v.Border};
  transition: padding 0.2s;
}}
.item:hover {{ padding-left: 0.5rem; }}
.item-main {{ flex: 1; min-width: 0; }}
.item-name {{
  display: block;
  font-size: 1.15rem;
  font-weight: 600;
  color: {v.Ink};
  text-decoration: none;
  margin-bottom: 0.25rem;
}}
.item-name:hover {{ color: {v.Accent}; }}
.item-desc {{
  font-size: 0.95rem;
  color: {v.Muted};
  font-weight: 300;
  line-height: 1.5;
}}
.item-meta {{
  font-family: {v.MonoFont};
  font-size: 0.75rem;
  color: #999;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.25rem;
  white-space: nowrap;
}}
.star {{ color: {v.Accent}; }}
footer {{
  margin-top: 6rem;
  padding-top: 2rem;
  border-top: 1px solid {v.Border};
  font-family: {v.MonoFont};
  font-size: 0.75rem;
  color: #999;
}}
@media (max-width: 500px) {{
  .container {{ padding: 4rem 1.5rem 3rem; }}
  .item {{ flex-direction: column; gap: 0.5rem; }}
  .item-meta {{ flex-direction: row; align-items: center; }}
}}
";

        var js = @"// Minimal portfolio — fade in on load
document.body.style.opacity = '0';
window.addEventListener('DOMContentLoaded', () => {
  document.body.style.transition = 'opacity 0.6s';
  document.body.style.opacity = '1';
});
";

        return new GeneratedPortfolio(html, css, js);
    }
}
