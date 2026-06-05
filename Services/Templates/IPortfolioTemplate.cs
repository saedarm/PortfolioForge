using PortfolioForge.Models;

namespace PortfolioForge.Services.Templates;

public interface IPortfolioTemplate
{
    string Name { get; }
    string Description { get; }
    int VariantCount { get; }
    GeneratedPortfolio Generate(GitHubUser user, List<GitHubRepo> repos, int variant);
}

public record GeneratedPortfolio(string Html, string Css, string Js);
