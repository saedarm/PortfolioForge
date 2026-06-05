using System.Net;
using System.Text;
using PortfolioForge.Models;

namespace PortfolioForge.Services.Templates;

public class SynthwaveTemplate : IPortfolioTemplate
{
    public string Name => "Synthwave";
    public string Description => "Neon grid. 1987 forever. Mostly safe for work.";
    public int VariantCount => 4;

    private record Variant(string Label, string Pink, string Cyan, string Purple, string BgStart, string BgMid, string BgEnd);

    private static readonly Variant[] _variants =
    {
        new("Classic", "#ff00d4", "#00f0ff", "#6a0dad", "#0d0221", "#2d0066", "#ff00d4"),  // pink/cyan
        new("Sunset",  "#ff5722", "#ffeb3b", "#3f51b5", "#1a0033", "#4a148c", "#ff5722"),  // orange/yellow Miami
        new("Toxic",   "#00ff88", "#ff00ff", "#1de9b6", "#0a0a0a", "#1a3a0a", "#00ff88"),  // green/magenta
        new("Vapor",   "#ffb6e6", "#a3e0ff", "#9c27b0", "#1a0033", "#3d1a4a", "#ffb6e6")   // pastel
    };

    public GeneratedPortfolio Generate(GitHubUser user, List<GitHubRepo> repos, int variant)
    {
        var v = _variants[Math.Abs(variant) % _variants.Length];
        var displayName = WebUtility.HtmlEncode(user.Name ?? user.Login);
        var bio = WebUtility.HtmlEncode(user.Bio ?? "executing dreams.exe");

        var repoCardsHtml = new StringBuilder();
        foreach (var r in repos.OrderByDescending(r => r.Stars).Take(12))
        {
            var name = WebUtility.HtmlEncode(r.Name);
            var desc = WebUtility.HtmlEncode(r.Description ?? "no description");
            var lang = WebUtility.HtmlEncode(r.Language ?? "—");
            var topics = r.Topics != null && r.Topics.Count > 0
                ? string.Join("", r.Topics.Take(3).Select(t => $@"<span class=""tag"">#{WebUtility.HtmlEncode(t)}</span>"))
                : "";
            repoCardsHtml.AppendLine($@"      <a class=""card"" href=""{r.HtmlUrl}"" target=""_blank"" rel=""noopener"">
        <div class=""card-glow""></div>
        <h3 class=""card-title"">{name}</h3>
        <p class=""card-desc"">{desc}</p>
        <div class=""card-tags"">{topics}</div>
        <div class=""card-stats"">
          <span>{lang}</span>
          <span>★ {r.Stars}</span>
          <span>⑂ {r.Forks}</span>
        </div>
      </a>");
        }

        var avatarHtml = !string.IsNullOrEmpty(user.AvatarUrl)
            ? $@"<div class=""avatar-frame""><img src=""{WebUtility.HtmlEncode(user.AvatarUrl)}"" alt=""{displayName}"" /></div>"
            : "";

        var socialList = SocialLinks.BuildList(user);
        var socialsHtml = new StringBuilder();
        foreach (var s in socialList)
        {
            socialsHtml.Append($@"<a class=""social-btn"" href=""{WebUtility.HtmlEncode(s.Url)}"" target=""_blank"">▸ {WebUtility.HtmlEncode(s.Label.ToUpper())}</a>");
        }

        var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{displayName} // {v.Label.ToLower()}</title>
  <meta property=""og:title"" content=""{displayName} // synthwave"">
  <meta property=""og:description"" content=""{bio}"">
  {(string.IsNullOrEmpty(user.AvatarUrl) ? "" : $@"<meta property=""og:image"" content=""{WebUtility.HtmlEncode(user.AvatarUrl)}"">")}
  <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
  <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
  <link href=""https://fonts.googleapis.com/css2?family=Orbitron:wght@400;700;900&family=Rajdhani:wght@300;500;700&display=swap"" rel=""stylesheet"">
  <link rel=""stylesheet"" href=""style.css"">
</head>
<body>
  <div class=""sun""></div>
  <div class=""grid""></div>
  <div class=""mountains""></div>
  <main>
    <header class=""hero"">
      {avatarHtml}
      <div class=""glitch-wrap"">
        <h1 class=""glitch"" data-text=""{displayName.ToUpper()}"">{displayName.ToUpper()}</h1>
      </div>
      <p class=""subtitle"">◢ {bio} ◣</p>
      <div class=""stats"">
        <div class=""stat""><div class=""stat-num"">{user.PublicRepos}</div><div class=""stat-lbl"">REPOSITORIES</div></div>
        <div class=""stat""><div class=""stat-num"">{user.Followers}</div><div class=""stat-lbl"">FOLLOWERS</div></div>
        <div class=""stat""><div class=""stat-num"">{repos.Sum(r => r.Stars)}</div><div class=""stat-lbl"">TOTAL STARS</div></div>
      </div>
      <div class=""socials"">{socialsHtml}</div>
    </header>
    <section>
      <h2>// FEATURED PROJECTS · {v.Label.ToUpper()}</h2>
      <div class=""cards"">
{repoCardsHtml}
      </div>
    </section>
    <footer>
      <p>POWERED BY PORTFOLIOFORGE · {DateTime.UtcNow:yyyy}</p>
    </footer>
  </main>
  <script src=""script.js""></script>
</body>
</html>";

        var css = $@"* {{ margin: 0; padding: 0; box-sizing: border-box; }}
:root {{
  --pink: {v.Pink};
  --cyan: {v.Cyan};
  --purple: {v.Purple};
}}
body {{
  background: linear-gradient(180deg, {v.BgStart} 0%, {v.BgMid} 60%, {v.BgEnd} 100%);
  color: #fff;
  font-family: 'Rajdhani', sans-serif;
  min-height: 100vh;
  overflow-x: hidden;
  position: relative;
}}
.sun {{
  position: fixed;
  bottom: 40vh;
  left: 50%;
  transform: translateX(-50%);
  width: 500px;
  height: 500px;
  border-radius: 50%;
  background: radial-gradient(circle at center, {v.Pink}aa 0%, {v.Pink} 40%, transparent 70%);
  filter: blur(20px);
  z-index: 0;
  pointer-events: none;
}}
.grid {{
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: 50vh;
  background-image:
    linear-gradient({v.Pink}80 1px, transparent 1px),
    linear-gradient(90deg, {v.Pink}80 1px, transparent 1px);
  background-size: 40px 40px;
  transform: perspective(400px) rotateX(60deg);
  transform-origin: center bottom;
  z-index: 0;
  pointer-events: none;
}}
.mountains {{
  position: fixed;
  bottom: 38vh;
  left: 0;
  right: 0;
  height: 80px;
  background: linear-gradient(180deg, transparent, {v.BgStart});
  clip-path: polygon(0% 100%, 8% 60%, 18% 80%, 28% 30%, 40% 70%, 52% 20%, 64% 65%, 76% 40%, 88% 75%, 100% 50%, 100% 100%);
  z-index: 0;
  pointer-events: none;
}}
main {{ position: relative; z-index: 1; padding: 4rem 2rem; max-width: 1200px; margin: 0 auto; }}
.hero {{ text-align: center; margin-bottom: 5rem; padding: 3rem 0; }}
.avatar-frame {{
  display: inline-block;
  padding: 4px;
  background: linear-gradient(135deg, {v.Pink}, {v.Cyan});
  border-radius: 50%;
  margin-bottom: 1.5rem;
  box-shadow: 0 0 30px {v.Pink}aa;
}}
.avatar-frame img {{
  width: 120px;
  height: 120px;
  border-radius: 50%;
  display: block;
  object-fit: cover;
  border: 2px solid {v.BgStart};
}}
.glitch-wrap {{ display: inline-block; }}
.glitch {{
  font-family: 'Orbitron', sans-serif;
  font-size: clamp(2.5rem, 9vw, 6rem);
  font-weight: 900;
  letter-spacing: 4px;
  color: #fff;
  position: relative;
  text-shadow:
    0 0 10px {v.Cyan},
    0 0 20px {v.Cyan},
    0 0 40px {v.Pink};
  animation: glow 3s ease-in-out infinite alternate;
}}
@keyframes glow {{
  from {{ text-shadow: 0 0 10px {v.Cyan}, 0 0 20px {v.Cyan}, 0 0 40px {v.Pink}; }}
  to {{ text-shadow: 0 0 14px {v.Pink}, 0 0 28px {v.Pink}, 0 0 56px {v.Cyan}; }}
}}
.subtitle {{
  font-size: 1.2rem;
  letter-spacing: 4px;
  margin-top: 1rem;
  color: {v.Cyan};
  text-shadow: 0 0 12px {v.Cyan};
}}
.stats {{ display: flex; justify-content: center; gap: 3rem; margin: 3rem 0; flex-wrap: wrap; }}
.stat {{
  border: 1px solid {v.Pink};
  padding: 1rem 2rem;
  background: {v.BgStart}99;
  box-shadow: 0 0 20px {v.Pink}66;
}}
.stat-num {{
  font-family: 'Orbitron', sans-serif;
  font-size: 2.5rem;
  font-weight: 700;
  color: {v.Cyan};
  text-shadow: 0 0 10px {v.Cyan};
}}
.stat-lbl {{ font-size: 0.75rem; letter-spacing: 3px; color: #fff; margin-top: 0.25rem; }}
.socials {{ display: flex; gap: 0.75rem; justify-content: center; flex-wrap: wrap; }}
.social-btn {{
  display: inline-block;
  padding: 0.6rem 1.4rem;
  background: transparent;
  color: {v.Pink};
  border: 2px solid {v.Pink};
  font-family: 'Orbitron', sans-serif;
  font-weight: 700;
  letter-spacing: 2px;
  text-decoration: none;
  font-size: 0.8rem;
  box-shadow: 0 0 20px {v.Pink}99, inset 0 0 20px {v.Pink}33;
  transition: all 0.2s;
}}
.social-btn:hover {{ background: {v.Pink}; color: #fff; box-shadow: 0 0 40px {v.Pink}ee; }}
h2 {{
  font-family: 'Orbitron', sans-serif;
  font-size: 1.5rem;
  letter-spacing: 4px;
  margin-bottom: 2rem;
  color: {v.Cyan};
  text-shadow: 0 0 12px {v.Cyan};
}}
.cards {{ display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem; }}
.card {{
  position: relative;
  background: {v.BgStart}b3;
  border: 1px solid {v.Cyan}66;
  padding: 1.5rem;
  text-decoration: none;
  color: #fff;
  transition: all 0.3s;
  backdrop-filter: blur(8px);
  overflow: hidden;
}}
.card:hover {{ border-color: {v.Pink}; box-shadow: 0 0 30px {v.Pink}80; transform: translateY(-4px); }}
.card-glow {{
  position: absolute;
  top: 0; left: 0; right: 0; height: 2px;
  background: linear-gradient(90deg, transparent, {v.Pink}, {v.Cyan}, transparent);
}}
.card-title {{
  font-family: 'Orbitron', sans-serif;
  font-size: 1.1rem;
  color: {v.Cyan};
  margin-bottom: 0.75rem;
  letter-spacing: 1px;
  text-shadow: 0 0 8px {v.Cyan};
}}
.card-desc {{ font-size: 0.95rem; color: #ccc; margin-bottom: 1rem; min-height: 2.8em; }}
.card-tags {{ display: flex; flex-wrap: wrap; gap: 0.4rem; margin-bottom: 1rem; }}
.tag {{ font-size: 0.7rem; padding: 2px 8px; background: {v.Pink}33; border: 1px solid {v.Pink}; color: {v.Pink}; letter-spacing: 1px; }}
.card-stats {{ display: flex; gap: 1rem; font-size: 0.75rem; letter-spacing: 2px; color: {v.Cyan}; border-top: 1px solid {v.Cyan}33; padding-top: 0.75rem; }}
footer {{ margin-top: 5rem; text-align: center; font-size: 0.75rem; letter-spacing: 4px; color: {v.Cyan}; opacity: 0.7; }}
";

        var js = @"const title = document.querySelector('.glitch');
if (title) {
  setInterval(() => {
    if (Math.random() < 0.05) {
      title.style.transform = `translate(${Math.random()*4-2}px, ${Math.random()*4-2}px)`;
      setTimeout(() => title.style.transform = '', 80);
    }
  }, 200);
}
";

        return new GeneratedPortfolio(html, css, js);
    }
}
