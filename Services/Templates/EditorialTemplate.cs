using System.Net;
using System.Text;
using PortfolioForge.Models;

namespace PortfolioForge.Services.Templates;

public class EditorialTemplate : IPortfolioTemplate
{
    public string Name => "Editorial";
    public string Description => "Magazine layout. Serif headlines. Asymmetric grid.";
    public int VariantCount => 4;

    private record Variant(string Label, string Paper, string Ink, string Muted, string Accent, string Border, string Serif, string GoogleFontUrl);

    private static readonly Variant[] _variants =
    {
        new("Quarterly",  "#f5f1e8", "#1a1a1a", "#444", "#b8860b", "#1a1a1a",
            "'Cormorant Garamond', Georgia, serif",
            "https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,400;0,500;0,700;1,400&family=Inter:wght@300;400;500&display=swap"),
        new("Vogue",      "#fafafa", "#0a0a0a", "#555", "#a30000", "#0a0a0a",
            "'Playfair Display', Georgia, serif",
            "https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,400;0,700;0,900;1,400&family=Inter:wght@300;400;500&display=swap"),
        new("Academic",   "#ebe6dc", "#2c2820", "#5e564a", "#6e1b1b", "#2c2820",
            "'EB Garamond', Georgia, serif",
            "https://fonts.googleapis.com/css2?family=EB+Garamond:ital,wght@0,400;0,500;0,700;1,400&family=Inter:wght@300;400;500&display=swap"),
        new("Modern",     "#fff8f0", "#2a1810", "#6b5544", "#0e7c66", "#2a1810",
            "'Fraunces', Georgia, serif",
            "https://fonts.googleapis.com/css2?family=Fraunces:ital,opsz,wght@0,9..144,400;0,9..144,600;0,9..144,800;1,9..144,400&family=Inter:wght@300;400;500&display=swap")
    };

    public GeneratedPortfolio Generate(GitHubUser user, List<GitHubRepo> repos, int variant)
    {
        var v = _variants[Math.Abs(variant) % _variants.Length];
        var displayName = WebUtility.HtmlEncode(user.Name ?? user.Login);
        var bio = WebUtility.HtmlEncode(user.Bio ?? "A developer with stories to tell.");
        var login = WebUtility.HtmlEncode(user.Login);

        var featured = repos.OrderByDescending(r => r.Stars).FirstOrDefault();
        var rest = repos.OrderByDescending(r => r.Stars).Skip(1).Take(12).ToList();

        var featuredHtml = featured == null ? "" : $@"
    <article class=""featured"">
      <div class=""featured-num"">01</div>
      <h2 class=""featured-title""><a href=""{featured.HtmlUrl}"" target=""_blank"">{WebUtility.HtmlEncode(featured.Name)}</a></h2>
      <p class=""featured-lede"">{WebUtility.HtmlEncode(featured.Description ?? "An ongoing study.")}</p>
      <div class=""featured-meta"">
        <span>{WebUtility.HtmlEncode(featured.Language ?? "Various")}</span>
        <span>·</span>
        <span>{featured.Stars} stars</span>
        <span>·</span>
        <span>{featured.Forks} forks</span>
      </div>
    </article>";

        var restHtml = new StringBuilder();
        var num = 2;
        foreach (var r in rest)
        {
            var name = WebUtility.HtmlEncode(r.Name);
            var desc = WebUtility.HtmlEncode(r.Description ?? "");
            var lang = WebUtility.HtmlEncode(r.Language ?? "Mixed");
            restHtml.AppendLine($@"      <article class=""piece"">
        <div class=""piece-num"">{num:D2}</div>
        <h3><a href=""{r.HtmlUrl}"" target=""_blank"">{name}</a></h3>
        <p>{desc}</p>
        <div class=""piece-meta"">{lang} <span class=""dot"">·</span> ★ {r.Stars}</div>
      </article>");
            num++;
        }

        // Byline avatar block
        var avatarHtml = !string.IsNullOrEmpty(user.AvatarUrl)
            ? $@"<img class=""byline-avatar"" src=""{WebUtility.HtmlEncode(user.AvatarUrl)}"" alt=""{displayName}"" />"
            : "";

        var socialList = SocialLinks.BuildList(user);
        var socialsHtml = new StringBuilder();
        foreach (var s in socialList)
        {
            socialsHtml.Append($@"<a href=""{WebUtility.HtmlEncode(s.Url)}"" target=""_blank"">{WebUtility.HtmlEncode(s.Label)}</a> ");
        }

        var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>{displayName} — The {v.Label}</title>
  <meta property=""og:title"" content=""{displayName} — The {v.Label}"">
  <meta property=""og:description"" content=""{bio}"">
  {(string.IsNullOrEmpty(user.AvatarUrl) ? "" : $@"<meta property=""og:image"" content=""{WebUtility.HtmlEncode(user.AvatarUrl)}"">")}
  <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
  <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
  <link href=""{v.GoogleFontUrl}"" rel=""stylesheet"">
  <link rel=""stylesheet"" href=""style.css"">
</head>
<body>
  <div class=""paper"">
    <header class=""masthead"">
      <div class=""masthead-top"">
        <span>VOL. {DateTime.UtcNow:yyyy}</span>
        <span>ISSUE №{DateTime.UtcNow.DayOfYear:D3}</span>
        <span>{DateTime.UtcNow:MMMM d, yyyy}</span>
      </div>
      {avatarHtml}
      <h1>{displayName}</h1>
      <p class=""dek"">{bio}</p>
      <div class=""masthead-meta"">
        {(string.IsNullOrEmpty(user.Location) ? "" : $"<span>{WebUtility.HtmlEncode(user.Location)}</span>")}
        <span>{user.PublicRepos} works</span>
        <span class=""socials"">{socialsHtml}</span>
      </div>
    </header>
    <main>
      <section class=""hero-section"">
        {featuredHtml}
      </section>
      <section class=""grid-section"">
        <h2 class=""section-h"">Other works</h2>
        <div class=""grid"">
{restHtml}
        </div>
      </section>
    </main>
    <footer>
      <p>—</p>
      <p>The {v.Label}. Composed by PortfolioForge.</p>
    </footer>
  </div>
  <script src=""script.js""></script>
</body>
</html>";

        var css = $@"* {{ margin: 0; padding: 0; box-sizing: border-box; }}
body {{
  background: {v.Paper};
  color: {v.Ink};
  font-family: 'Inter', system-ui, sans-serif;
  font-size: 16px;
  line-height: 1.6;
}}
.paper {{ max-width: 1100px; margin: 0 auto; padding: 4rem 2rem; }}
.masthead {{
  text-align: center;
  padding-bottom: 3rem;
  border-bottom: 2px solid {v.Border};
  margin-bottom: 4rem;
}}
.masthead-top {{
  display: flex;
  justify-content: space-between;
  font-size: 0.75rem;
  letter-spacing: 3px;
  text-transform: uppercase;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid {v.Border};
}}
.byline-avatar {{
  width: 96px;
  height: 96px;
  border-radius: 50%;
  object-fit: cover;
  margin: 0 auto 1.5rem;
  display: block;
  filter: grayscale(1);
  border: 3px double {v.Border};
  padding: 4px;
  background: {v.Paper};
}}
h1 {{
  font-family: {v.Serif};
  font-size: clamp(3rem, 9vw, 6.5rem);
  font-weight: 500;
  line-height: 1;
  letter-spacing: -2px;
  margin-bottom: 1.5rem;
}}
.dek {{
  font-family: {v.Serif};
  font-style: italic;
  font-size: 1.5rem;
  max-width: 600px;
  margin: 0 auto 2rem;
  color: {v.Muted};
}}
.masthead-meta {{
  font-size: 0.85rem;
  letter-spacing: 1px;
  display: flex;
  gap: 1.5rem;
  justify-content: center;
  flex-wrap: wrap;
  text-transform: uppercase;
}}
.socials a {{
  color: {v.Ink};
  text-decoration: none;
  border-bottom: 1px solid {v.Ink};
  padding-bottom: 1px;
  margin-right: 0.5rem;
}}
.socials a:hover {{ color: {v.Accent}; border-color: {v.Accent}; }}
.hero-section {{ margin-bottom: 5rem; }}
.featured {{
  display: grid;
  grid-template-columns: 80px 1fr;
  gap: 2rem;
  padding: 2rem 0;
  border-top: 1px solid {v.Border};
  border-bottom: 1px solid {v.Border};
}}
.featured-num {{
  font-family: {v.Serif};
  font-size: 4rem;
  font-weight: 500;
  line-height: 1;
  color: {v.Accent};
}}
.featured-title {{
  font-family: {v.Serif};
  font-size: clamp(2rem, 5vw, 3.5rem);
  font-weight: 700;
  line-height: 1.1;
  margin-bottom: 1rem;
  letter-spacing: -1px;
}}
.featured-title a {{ color: {v.Ink}; text-decoration: none; }}
.featured-title a:hover {{ text-decoration: underline; }}
.featured-lede {{
  font-family: {v.Serif};
  font-size: 1.4rem;
  line-height: 1.5;
  font-style: italic;
  color: {v.Muted};
  margin-bottom: 1rem;
}}
.featured-lede::first-letter {{
  font-size: 4rem;
  float: left;
  line-height: 0.9;
  font-weight: 700;
  padding-right: 0.5rem;
  padding-top: 0.3rem;
  font-style: normal;
}}
.featured-meta {{
  font-size: 0.8rem;
  letter-spacing: 2px;
  text-transform: uppercase;
  color: {v.Muted};
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}}
.section-h {{
  font-family: {v.Serif};
  font-size: 2rem;
  font-style: italic;
  margin-bottom: 2rem;
  text-align: center;
  font-weight: 500;
}}
.grid {{
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 2.5rem;
}}
.piece {{ border-top: 1px solid {v.Border}; padding-top: 1.5rem; }}
.piece-num {{ font-family: {v.Serif}; font-size: 1.5rem; color: {v.Accent}; margin-bottom: 0.5rem; }}
.piece h3 {{ font-family: {v.Serif}; font-size: 1.6rem; font-weight: 700; line-height: 1.2; margin-bottom: 0.75rem; }}
.piece h3 a {{ color: {v.Ink}; text-decoration: none; }}
.piece h3 a:hover {{ text-decoration: underline; color: {v.Accent}; }}
.piece p {{ font-size: 0.95rem; color: {v.Muted}; margin-bottom: 1rem; min-height: 3em; }}
.piece-meta {{ font-size: 0.75rem; letter-spacing: 2px; text-transform: uppercase; color: {v.Muted}; }}
.dot {{ color: {v.Accent}; }}
footer {{
  margin-top: 6rem;
  text-align: center;
  font-size: 0.85rem;
  color: {v.Muted};
  font-style: italic;
}}
footer p:first-child {{ font-size: 2rem; margin-bottom: 1rem; color: {v.Accent}; }}
@media (max-width: 600px) {{
  .paper {{ padding: 2rem 1rem; }}
  .masthead-top {{ flex-direction: column; gap: 0.5rem; }}
  .featured {{ grid-template-columns: 1fr; gap: 1rem; }}
}}
";

        var js = $@"console.log('%cThe {v.Label}', 'font-family: Georgia, serif; font-size: 24px; font-style: italic;');
";
        return new GeneratedPortfolio(html, css, js);
    }
}
