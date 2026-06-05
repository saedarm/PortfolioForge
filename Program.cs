using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PortfolioForge;
using PortfolioForge.Services;
using PortfolioForge.Services.Templates;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://api.github.com/") });
builder.Services.AddScoped<GitHubService>();
builder.Services.AddScoped<PortfolioGenerator>();

// Register all templates. Add new ones here.
builder.Services.AddScoped<IPortfolioTemplate, BrutalistTemplate>();
builder.Services.AddScoped<IPortfolioTemplate, TerminalTemplate>();
builder.Services.AddScoped<IPortfolioTemplate, EditorialTemplate>();
builder.Services.AddScoped<IPortfolioTemplate, SynthwaveTemplate>();
builder.Services.AddScoped<IPortfolioTemplate, MinimalTemplate>();

await builder.Build().RunAsync();
