
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Octokit;
using Microsoft.SemanticKernel;
using System.Configuration;
using SKPrompt.Helpers;

// if (args.Length == 0)
// {
//     Console.WriteLine("Please provide a GitHub repository URL.");
//     return;
// }
// string repoUrl = args[0];
var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");

/* example URLS
https://github.com/Rickcau/sk-sports-assistant
https://github.com/microsoft/semantic-kernel
*/

var url = "https://github.com/Rickcau/sk-sports-assistant";

string readmeContent = await GetReadmeContent(url);
var builder = Kernel.CreateBuilder();
// Add the endpoint details.
builder.Services.AddAzureOpenAIChatCompletion(
   deploymentName: openAiDeployment!,
   endpoint: openAiUri!,
   apiKey: openAiApiKey!);

SKPromptHelper skPromptHelper = new();
var kernel = builder.Build();

var repoAnalysisJSON = await skPromptHelper.GetRepoReadmeDetailsAsync(kernel, readmeContent, url);

if (readmeContent != null)
{
    Console.WriteLine("README.md content:");
    Console.WriteLine(readmeContent);
}
else
{
    Console.WriteLine("Failed to retrieve README.md content.");
}

async Task<string> GetReadmeContent(string repoUrl)
{
    try
    {
        var (owner, repo) = ParseGitHubUrl(repoUrl);
        
        // Try using GitHub API first
        var client = new GitHubClient(new ProductHeaderValue("GitHubReadmeDownloader"));
        var readme = await client.Repository.Content.GetReadme(owner, repo);
        return readme.Content;
    }
    catch (NotFoundException)
    {
        // If API fails, try direct download
        return await DownloadReadmeDirectly(repoUrl);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return null;
    }
}

async Task<string> DownloadReadmeDirectly(string repoUrl)
{
    using (var httpClient = new HttpClient())
    {
        string readmeUrl = $"{repoUrl}/raw/main/README.md";
        try
        {
            return await httpClient.GetStringAsync(readmeUrl);
        }
        catch
        {
            readmeUrl = $"{repoUrl}/raw/master/README.md";
            return await httpClient.GetStringAsync(readmeUrl);
        }
    }
}

(string owner, string repo) ParseGitHubUrl(string url)
{
    var parts = new Uri(url).AbsolutePath.Trim('/').Split('/');
    if (parts.Length < 2)
    {
        throw new ArgumentException("Invalid GitHub URL");
    }
    return (parts[0], parts[1]);
}