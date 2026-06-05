# PortfolioForge

Turn any GitHub username into a downloadable developer portfolio. Five themes, four variants each, twenty distinct portfolios in twenty clicks. Export as a ZIP of static HTML, CSS, and JS. Deploy anywhere.

Built in Blazor WebAssembly on .NET 10. Runs entirely in the browser — no backend, no database, no data leaving your tab.
<img width="771" height="866" alt="image" src="https://github.com/user-attachments/assets/9889bf5e-96c7-45b3-8ad2-c51164113ca0" />


## What it does

You enter a GitHub username (or paste the full profile URL). PortfolioForge fetches the public profile and repo data from GitHub's REST API, then generates a complete single-page portfolio. You can flip through themes, click variant dots to swap palettes and fonts, or hit Randomize to shuffle the whole thing. When you find one you like, click Export ZIP. You get four files:

- `index.html`
- `style.css`
- `script.js`
- `README.md` (deployment instructions)

Drop those on any static host. Done. No build step, no framework to learn, no dependencies, no account, no subscription.

## Themes

Five themes, each with four variants for a total of twenty distinct portfolios:

| Theme       | Variants                                          | Vibe                                          |
|-------------|---------------------------------------------------|------------------------------------------------|
| Brutalist   | Cream, Inverted, Blueprint, Riot                  | Heavy borders, monospace, no apologies         |
| Terminal    | Phosphor, Amber, IBM, Modern                      | CRT terminal with scanlines and a blinking cursor |
| Editorial   | Quarterly, Vogue, Academic, Modern                | Magazine layout with four serif pairings       |
| Synthwave   | Classic, Sunset, Toxic, Vapor                     | Neon grid, glow effects, 1987 forever          |
| Minimal     | Newsreader, Bricolage, Fraunces, Sepia Lora       | Lots of whitespace, refined typography         |

Adding a new theme is one C# file. See [Adding a theme](#adding-a-theme) below.

## Quick start

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
git clone https://github.com/saedarm/portfolioforge.git
cd portfolioforge/src/PortfolioForge
dotnet run
```

Open `https://localhost:7283` (or whatever port `launchSettings.json` assigns).

Type a GitHub username, hit Forge It, click around.

## Project structure

```
PortfolioForge/
├── src/
│   └── PortfolioForge/
│       ├── Models/                    GitHub API response models
│       ├── Pages/
│       │   └── Index.razor            Main UI
│       ├── Services/
│       │   ├── GitHubService.cs       API client, token handling
│       │   ├── PortfolioGenerator.cs  Theme registry, ZIP export
│       │   └── Templates/
│       │       ├── IPortfolioTemplate.cs    Template interface
│       │       ├── SocialLinks.cs           Shared helper
│       │       ├── BrutalistTemplate.cs
│       │       ├── TerminalTemplate.cs
│       │       ├── EditorialTemplate.cs
│       │       ├── SynthwaveTemplate.cs
│       │       └── MinimalTemplate.cs
│       ├── wwwroot/
│       │   ├── index.html
│       │   ├── css/app.css
│       │   ├── js/download.js
│       │   └── staticwebapp.config.json
│       ├── Program.cs                 DI registration
│       ├── App.razor
│       └── PortfolioForge.csproj
└── .github/workflows/                 Azure SWA auto-deploy
```

## Adding a theme

Each theme is a single C# file implementing `IPortfolioTemplate`:

```csharp
public interface IPortfolioTemplate
{
    string Name { get; }
    string Description { get; }
    int VariantCount { get; }
    GeneratedPortfolio Generate(GitHubUser user, List<GitHubRepo> repos, int variant);
}
```

A `Generate` call takes the user data, the repos, and a variant index, and returns three strings — HTML, CSS, and JS. Look at any of the existing templates for the pattern. The cleanest one to study is `MinimalTemplate.cs`.

The variant system uses a private record array inside each template:

```csharp
private record Variant(string Label, string Bg, string Ink, string Accent, ...);

private static readonly Variant[] _variants =
{
    new("Cream",     "#f4f1ea", "#0a0a0a", "#ff3c00", ...),
    new("Inverted",  "#0a0a0a", "#f4f1ea", "#ff5252", ...),
    // add more here
};
```

The HTML structure stays consistent across variants — only colors and (sometimes) fonts change. This keeps the bug surface small.

Once your template is written:

1. Drop the file in `Services/Templates/`
2. Register it in `Program.cs` alongside the others:
   ```csharp
   builder.Services.AddScoped<IPortfolioTemplate, YourTemplate>();
   ```
3. Build. Randomize will pick it up automatically.

PRs welcome. Bonus points if your theme has a strong opinion about something.

## Tech stack

- **Blazor WebAssembly** on **.NET 10** (LTS through 2028)
- **C# 14** language features
- Standard library only — no third-party NuGet packages beyond the Blazor host. `HttpClient`, `System.Text.Json`, `System.IO.Compression`. That's it.
- **Tailwind, React, etc.: none of the above.** The output is vanilla HTML and CSS.

## Why no backend

The entire app runs in your browser. There's an optional input for a GitHub personal access token to bump your rate limit from 60 to 5,000 requests per hour. That token goes from your browser directly to `api.github.com`. There's no middleware that could log it, no database that stores it, nothing on a server. When you close the tab, the token is gone.

This is the actual point of going fully client-side for this kind of tool. The privacy story isn't a policy you have to trust — it's just how the architecture works.


## Roadmap

Things on the list, in rough priority order:

- [ ] **Pinned repos.** GitHub's pinned-repos feature only exposes via GraphQL, not REST. Adding GraphQL would let users surface the projects they actually want highlighted instead of whatever's starred or recent.
- [ ] **URL state.** Encode template and variant in the URL hash so a specific portfolio is shareable.
- [ ] **Deep links.** Click a link, land on PortfolioForge with that exact theme and variant preloaded.
- [ ] **More themes.** Five was the minimum I felt okay shipping with. The next four should be community-contributed.
- [ ] **Profile README integration.** GitHub users can have a special `username/username` repo whose README renders on their profile. Currently ignored — should be parsed and included.
- [ ] **Top languages aggregation.** Roll up `language` across all repos into a small "primary languages" widget.

## Contributing

Open issues for bugs or feature ideas. Open PRs for themes and small fixes. For anything bigger, file an issue first so we can talk about scope.

Code style: follow what's already there. Nothing unusual. Standard .NET conventions.

## Acknowledgments

PortfolioForge exists because of Emma Bostian's [developer-portfolios](https://github.com/emmabostian/developer-portfolios) repo and the [companion site](https://6e87v.hatchboxapp.com) that makes browsing 1,600+ portfolios actually feasible. If you're a developer thinking about your own portfolio, start there.

Originally inspired by an Ali Spittel tweet suggesting developers share their work more freely. This is one small attempt to lower the barrier to doing that.

## License

MIT. See [LICENSE](LICENSE).
