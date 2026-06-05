using System.Net;
using System.Text;
using PortfolioForge.Models;

namespace PortfolioForge.Services.Templates;

public class BrutalistTemplate : IPortfolioTemplate
{
    public string Name => "Brutalist";
    public string Description => "Raw concrete. Heavy borders. No apologies.";
    public int VariantCount => 4;

    private record Variant(string Label, string Paper, string Ink, string Accent, string AccentText, bool DarkMode);

    private static readonly Variant[] _variants =
    {
        new("Cream",       "#f4f1ea", "#0a0a0a", "#ff3c00", "#0a0a0a", false), // original
        new("Inverted",    "#0a0a0a", "#f4f1ea", "#ff5252", "#0a0a0a", true),
        new("Blueprint",   "#e8eef3", "#0c2a4d", "#f5b800", "#0c2a4d", false),
        new("Riot",        "#fdf6e3", "#c0392b", "#0a0a0a", "#fdf6e3", false)
    };

    public GeneratedPortfolio Generate(GitHubUser user, List<GitHubRepo> repos, int variant)
    {
        var v = _variants[Math.Abs(variant) % _variants.Length];
        var displayName = WebUtility.HtmlEncode(user.Name ?? user.Login);
        var bio = WebUtility.HtmlEncode(user.Bio ?? "Builder of things.");

        var repoCardsHtml = new StringBuilder();
        foreach (var r in repos.OrderByDescending(r => r.Stars).Take(20))
        {
            var name = WebUtility.HtmlEncode(r.Name).ToUpper();
            var desc = WebUtility.HtmlEncode(r.Description ?? "");
            var lang = WebUtility.HtmlEncode(r.Language ?? "—");
            repoCardsHtml.AppendLine($@"      <a class=""repo"" href=""{r.HtmlUrl}"" target=""_blank"" rel=""noopener"">
        <div class=""repo-head"">
          <span class=""repo-name"">{name}</span>
          <span class=""repo-stars"">★ {r.Stars}</span>
        </div>
        <p class=""repo-desc"">{desc}</p>
        <div class=""repo-foot"">
          <span class=""repo-lang"">{lang}</span>
          <span class=""repo-forks"">⑂ {r.Forks}</span>
        </div>
      </a>");
        }

        var socialsHtml = SocialLinks.RenderHtml(user, "socials");
        var avatarBlock = !string.IsNullOrEmpty(user.AvatarUrl)
            ? $@"<img class=""avatar"" src=""{WebUtility.HtmlEncode(user.AvatarUrl)}"" alt=""{displayName}"" />"
            : @"<div class=""avatar avatar-placeholder""></div>";

        var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{displayName} — Portfolio</title>
  <meta property=""og:title"" content=""{displayName} — Portfolio"">
  <meta property=""og:description"" content=""{bio}"">
  {(string.IsNullOrEmpty(user.AvatarUrl) ? "" : $@"<meta property=""og:image"" content=""{WebUtility.HtmlEncode(user.AvatarUrl)}"">")}
  <link rel=""stylesheet"" href=""style.css"">
</head>
<body>
  <header class=""hero"">
    <div class=""hero-grid"">
      <div class=""hero-left"">
        {avatarBlock}
        <h1>{displayName.ToUpper()}</h1>
        <p class=""tag"">{bio}</p>
        <div class=""meta"">
          <span>{WebUtility.HtmlEncode(user.Location ?? "EARTH").ToUpper()}</span>
          <span>·</span>
          <span>{user.PublicRepos} REPOS</span>
          <span>·</span>
          <span>{user.Followers} FOLLOWERS</span>
        </div>
        {socialsHtml}
      </div>
      <div class=""hero-right"">
        <div class=""logo-block"">
          <div class=""logo-line""></div>
          <div class=""logo-line""></div>
          <div class=""logo-line""></div>
          <div class=""logo-id"">{WebUtility.HtmlEncode(user.Login).ToUpper()}/{DateTime.UtcNow:yyyy}</div>
        </div>
      </div>
    </div>
  </header>
  <main>
    <section>
      <h2>SELECTED WORK / {v.Label.ToUpper()}</h2>
      <div class=""repos"">
{repoCardsHtml}
      </div>
    </section>
  </main>
  <footer>
    <p>BUILT WITH PORTFOLIOFORGE / {DateTime.UtcNow:yyyy.MM.dd}</p>
  </footer>
  <script src=""script.js""></script>
</body>
</html>";

        var css = $@"* {{ margin: 0; padding: 0; box-sizing: border-box; }}
:root {{
  --ink: {v.Ink};
  --paper: {v.Paper};
  --accent: {v.Accent};
  --accent-text: {v.AccentText};
  --line: 3px solid var(--ink);
}}
body {{
  font-family: 'JetBrains Mono', 'Courier New', monospace;
  background: var(--paper);
  color: var(--ink);
  line-height: 1.5;
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}}
.hero {{
  border: var(--line);
  padding: 3rem 2rem;
  margin-bottom: 2rem;
  background: var(--paper);
}}
.hero-grid {{
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 2rem;
  align-items: center;
}}
@media (max-width: 720px) {{ .hero-grid {{ grid-template-columns: 1fr; }} }}
.avatar {{
  width: 96px;
  height: 96px;
  display: block;
  margin-bottom: 1.25rem;
  border: var(--line);
  filter: grayscale(0.8) contrast(1.2);
  object-fit: cover;
}}
.avatar-placeholder {{ background: var(--ink); }}
h1 {{
  font-size: clamp(2.5rem, 8vw, 5rem);
  font-weight: 900;
  letter-spacing: -2px;
  line-height: 0.95;
  margin-bottom: 1rem;
  word-break: break-word;
}}
.tag {{
  font-size: 1.1rem;
  margin-bottom: 1.5rem;
  max-width: 500px;
}}
.meta {{
  font-size: 0.85rem;
  margin-bottom: 1.5rem;
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  letter-spacing: 1px;
}}
.socials {{
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}}
.socials a {{
  display: inline-block;
  background: var(--ink);
  color: var(--paper);
  padding: 0.7rem 1.2rem;
  text-decoration: none;
  font-weight: 700;
  letter-spacing: 2px;
  font-size: 0.8rem;
  border: var(--line);
  transition: transform 0.1s;
  text-transform: uppercase;
}}
.socials a:hover {{
  background: var(--accent);
  color: var(--accent-text);
  transform: translate(-2px, -2px);
  box-shadow: 4px 4px 0 var(--ink);
}}
.logo-block {{
  border: var(--line);
  padding: 1.5rem;
  background: var(--ink);
  color: var(--paper);
}}
.logo-line {{ height: 6px; background: var(--paper); margin-bottom: 0.6rem; }}
.logo-line:nth-child(1) {{ width: 100%; }}
.logo-line:nth-child(2) {{ width: 70%; background: var(--accent); }}
.logo-line:nth-child(3) {{ width: 40%; }}
.logo-id {{ margin-top: 1rem; font-size: 0.75rem; letter-spacing: 2px; }}
h2 {{
  font-size: 1.5rem;
  letter-spacing: 4px;
  margin-bottom: 1.5rem;
  padding-bottom: 0.5rem;
  border-bottom: var(--line);
}}
.repos {{
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 0;
}}
.repo {{
  border: var(--line);
  margin: -1.5px;
  padding: 1.5rem;
  text-decoration: none;
  color: var(--ink);
  background: var(--paper);
  transition: background 0.1s;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-height: 180px;
}}
.repo:hover {{ background: var(--accent); color: var(--accent-text); }}
.repo-head {{ display: flex; justify-content: space-between; align-items: flex-start; gap: 0.5rem; margin-bottom: 0.5rem; }}
.repo-name {{ font-weight: 700; font-size: 1rem; letter-spacing: 1px; }}
.repo-stars {{ font-size: 0.85rem; white-space: nowrap; }}
.repo-desc {{ font-size: 0.85rem; margin: 0.5rem 0 1rem; flex: 1; }}
.repo-foot {{ display: flex; justify-content: space-between; font-size: 0.75rem; letter-spacing: 1px; border-top: 2px solid currentColor; padding-top: 0.5rem; }}
footer {{
  margin-top: 4rem;
  padding-top: 2rem;
  border-top: var(--line);
  font-size: 0.75rem;
  letter-spacing: 2px;
}}
";

        var js = @"// Brutalist portfolio — generated by PortfolioForge
document.querySelectorAll('.repo').forEach(card => {
  card.addEventListener('mouseenter', () => card.style.transform = 'translate(-2px,-2px)');
  card.addEventListener('mouseleave', () => card.style.transform = 'none');
});
";

        return new GeneratedPortfolio(html, css, js);
    }
}
