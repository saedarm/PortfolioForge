using System.Net;
using System.Text;
using PortfolioForge.Models;

namespace PortfolioForge.Services.Templates;

public class TerminalTemplate : IPortfolioTemplate
{
    public string Name => "Terminal";
    public string Description => "Green phosphor CRT. Looks like 1984 in a good way.";
    public int VariantCount => 4;

    private record Variant(string Label, string Bg, string Fg, string Highlight, string Dim, string Accent, string Link);

    private static readonly Variant[] _variants =
    {
        new("Phosphor",  "#0a0f0a", "#33ff33", "#80ff80", "#6f8f6f", "#ffe066", "#66ffff"),  // green
        new("Amber",     "#0f0a00", "#ffb000", "#ffd980", "#996600", "#ff66cc", "#ffffff"),  // amber CRT
        new("IBM",       "#000018", "#8cb4ff", "#c4d4ff", "#5470b0", "#ffe066", "#ff66cc"),  // blue terminal
        new("Modern",    "#1a1a1a", "#e5e5e5", "#ffffff", "#777777", "#00ff88", "#66c8ff")   // light-on-dark modern
    };

    public GeneratedPortfolio Generate(GitHubUser user, List<GitHubRepo> repos, int variant)
    {
        var v = _variants[Math.Abs(variant) % _variants.Length];
        var displayName = WebUtility.HtmlEncode(user.Name ?? user.Login);
        var bio = WebUtility.HtmlEncode(user.Bio ?? "Developer.");
        var login = WebUtility.HtmlEncode(user.Login);

        var repoRowsHtml = new StringBuilder();
        var idx = 1;
        foreach (var r in repos.OrderByDescending(r => r.PushedAt).Take(15))
        {
            var name = WebUtility.HtmlEncode(r.Name);
            var desc = WebUtility.HtmlEncode(r.Description ?? "no description");
            var lang = WebUtility.HtmlEncode(r.Language ?? "txt");
            repoRowsHtml.AppendLine($@"        <div class=""row"">
          <span class=""row-idx"">[{idx:D2}]</span>
          <a class=""row-name"" href=""{r.HtmlUrl}"" target=""_blank"">{name}</a>
          <span class=""row-lang"">{lang}</span>
          <span class=""row-stars"">★{r.Stars}</span>
          <div class=""row-desc"">└─ {desc}</div>
        </div>");
            idx++;
        }

        // Build social commands
        var socialList = SocialLinks.BuildList(user);
        var socialsHtml = new StringBuilder();
        foreach (var s in socialList)
        {
            socialsHtml.AppendLine($@"          <a href=""{WebUtility.HtmlEncode(s.Url)}"" target=""_blank"" class=""link"">→ {WebUtility.HtmlEncode(s.Label.ToLower())}: {WebUtility.HtmlEncode(s.Url.Replace("https://", "").Replace("http://", "").Replace("mailto:", ""))}</a><br>");
        }

        var avatarHtml = !string.IsNullOrEmpty(user.AvatarUrl)
            ? $@"<img class=""ascii-avatar"" src=""{WebUtility.HtmlEncode(user.AvatarUrl)}"" alt="""" />"
            : "";

        var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{login}@portfolio:~$</title>
  <meta property=""og:title"" content=""{displayName} — Terminal"">
  <meta property=""og:description"" content=""{bio}"">
  {(string.IsNullOrEmpty(user.AvatarUrl) ? "" : $@"<meta property=""og:image"" content=""{WebUtility.HtmlEncode(user.AvatarUrl)}"">")}
  <link rel=""stylesheet"" href=""style.css"">
</head>
<body>
  <div class=""crt"">
    <div class=""scanlines""></div>
    <div class=""terminal"">
      <div class=""bar"">
        <span class=""dot dot-r""></span>
        <span class=""dot dot-y""></span>
        <span class=""dot dot-g""></span>
        <span class=""bar-title"">{login}@portfolio:~/dev — {v.Label.ToLower()}</span>
      </div>
      <div class=""screen"">
        <div class=""line"">$ whoami</div>
        <div class=""out user-block"">
          {avatarHtml}
          <div>
            <span class=""hl"">{displayName}</span><br>
            {bio}<br>
            <span class=""dim"">location:</span> {WebUtility.HtmlEncode(user.Location ?? "/dev/null")}<br>
            <span class=""dim"">repos:</span> {user.PublicRepos}  <span class=""dim"">followers:</span> {user.Followers}
          </div>
        </div>
        <div class=""line"">$ ls -la ./projects | sort -t pushed</div>
        <div class=""out repos"">
{repoRowsHtml}
        </div>
        <div class=""line"">$ cat /etc/contact</div>
        <div class=""out"">
{socialsHtml}
        </div>
        <div class=""line"">$ <span class=""cursor"">_</span></div>
      </div>
    </div>
  </div>
  <script src=""script.js""></script>
</body>
</html>";

        var css = $@"* {{ margin: 0; padding: 0; box-sizing: border-box; }}
body {{
  background: #000;
  min-height: 100vh;
  font-family: 'IBM Plex Mono', 'Courier New', monospace;
  font-size: 14px;
  padding: 1.5rem;
  color: {v.Fg};
  overflow-x: hidden;
}}
.crt {{ position: relative; max-width: 920px; margin: 0 auto; }}
.scanlines {{
  position: fixed;
  inset: 0;
  pointer-events: none;
  background: repeating-linear-gradient(
    0deg,
    rgba(255,255,255,0.03),
    rgba(255,255,255,0.03) 1px,
    transparent 1px,
    transparent 3px
  );
  z-index: 9999;
}}
.terminal {{
  background: {v.Bg};
  border: 1px solid {v.Dim};
  border-radius: 8px;
  box-shadow: 0 0 60px {v.Fg}33, inset 0 0 80px #0008;
  overflow: hidden;
}}
.bar {{
  background: #0d1a0d;
  padding: 0.6rem 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  border-bottom: 1px solid {v.Dim};
}}
.dot {{ width: 12px; height: 12px; border-radius: 50%; display: inline-block; }}
.dot-r {{ background: #ff5f56; }}
.dot-y {{ background: #ffbd2e; }}
.dot-g {{ background: #27c93f; }}
.bar-title {{ color: {v.Dim}; margin-left: 0.75rem; font-size: 0.85rem; }}
.screen {{
  padding: 1.5rem;
  min-height: 70vh;
  line-height: 1.6;
  text-shadow: 0 0 4px {v.Fg}80;
}}
.user-block {{ display: flex; gap: 1rem; align-items: flex-start; }}
.ascii-avatar {{
  width: 80px;
  height: 80px;
  filter: grayscale(1) contrast(1.4) brightness(1.1);
  border: 1px solid {v.Dim};
  image-rendering: pixelated;
  opacity: 0.85;
}}
.line {{ margin: 1rem 0 0.5rem; color: {v.Highlight}; }}
.out {{ margin-bottom: 1rem; padding-left: 0.5rem; }}
.hl {{ color: #fff; font-weight: 700; }}
.dim {{ color: {v.Dim}; }}
.link {{ color: {v.Link}; text-decoration: underline; }}
.link:hover {{ background: {v.Link}; color: {v.Bg}; text-decoration: none; }}
.repos {{ display: flex; flex-direction: column; gap: 0.6rem; }}
.row {{
  display: grid;
  grid-template-columns: 40px 1fr auto auto;
  gap: 0.75rem;
  align-items: baseline;
  padding: 0.25rem 0;
}}
.row-idx {{ color: {v.Dim}; }}
.row-name {{ color: {v.Highlight}; text-decoration: none; font-weight: 600; }}
.row-name:hover {{ background: {v.Highlight}; color: {v.Bg}; }}
.row-lang {{ color: {v.Accent}; font-size: 0.85rem; }}
.row-stars {{ color: {v.Fg}; font-size: 0.85rem; }}
.row-desc {{ grid-column: 1 / -1; color: {v.Dim}; font-size: 0.85rem; padding-left: 0.5rem; }}
.cursor {{
  display: inline-block;
  background: {v.Fg};
  color: {v.Bg};
  animation: blink 1s steps(2) infinite;
  width: 0.6em;
  text-align: center;
}}
@keyframes blink {{ 50% {{ opacity: 0; }} }}
@media (max-width: 600px) {{
  .row {{ grid-template-columns: 32px 1fr auto; }}
  .row-lang {{ display: none; }}
  .user-block {{ flex-direction: column; }}
}}
";

        var js = @"// Terminal portfolio — typing effect on load
window.addEventListener('DOMContentLoaded', () => {
  const lines = document.querySelectorAll('.line');
  lines.forEach((l, i) => {
    l.style.opacity = '0';
    setTimeout(() => {
      l.style.transition = 'opacity 0.3s';
      l.style.opacity = '1';
    }, 200 + i * 280);
  });
});
";

        return new GeneratedPortfolio(html, css, js);
    }
}
