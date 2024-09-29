using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using Repo.Models;

namespace SKPrompt.Helpers;
    public class SKPromptHelper
    {

       private string _promptReadmeAnalysis = @"
        README Content: {{$readmeContent}}
        Repository URL: {{$repoUrl}}

        You are an AI assistant specialized in analyzing GitHub repository README files. Your task is to extract key information from the given README content and repository URL, focusing on five main aspects:

        1. A concise description of the repository
        2. The intended use or purpose of the repository
        3. Relevant tags or keywords that describe the repository's content, technology, or domain
        4. The author or primary contributor of the repository
        5. The full URL of the repository

        IMPORTANT: Only use information explicitly stated in the provided README content or repository URL. Do not infer, guess, or generate any information that is not directly present in these sources.

        Approach this task as follows:

        1. Carefully read through the entire README content provided.
        2. Look for sections or information that typically describe the repository, such as:
        - Introduction or opening paragraphs
        - ""About"" sections
        - Project descriptions
        - Taglines or slogans
        3. Identify information about the repository's intended use, including:
        - Use cases
        - Target audience
        - Problems it solves
        - Features and capabilities
        4. Extract relevant tags or keywords that describe:
        - Technologies used (e.g., programming languages, frameworks, tools)
        - Domain or field of application (e.g., web development, machine learning, data analysis)
        - Key features or concepts (e.g., API, database, authentication)
        5. Look for information about the author or primary contributor. This might be found in:
        - The README header
        - A ""Contributors"" or ""Author"" section
        - Copyright notices
        6. Use the provided repository URL. If not provided or empty, use ""Unknown"".
        7. Synthesize this information into concise summaries, a list of tags, and identify the author and URL.
        8. If the README lacks clear information about any of the required aspects, use ""Not specified in README"" for that field.
        9. Ignore installation instructions, contribution guidelines, and other technical details unless they directly relate to the required information.
        10. Use neutral, objective language in your summary.

        Format your response as a JSON object with the following structure:
        [JSON]
        {
        ""description"": ""A concise description of the repository, or 'Not specified in README' if not found"",
        ""intendedUse"": ""An explanation of the repository's intended use or purpose, or 'Not specified in README' if not found"",
        ""tags"": [""tag1"", ""tag2"", ""tag3"", ...],
        ""author"": ""The identified author or 'Not specified in README'"",
        ""url"": ""The full repository URL or 'Unknown'""
        }
        [JSON END]

        Ensure that the JSON is valid and properly escaped. Do not include any explanatory text outside of the JSON object. Limit the number of tags to a maximum of 10, focusing on the most relevant and descriptive ones that are explicitly mentioned in the README. If no relevant tags are found, use an empty array [].

        [Examples for JSON Output]
        {
        ""description"": ""A lightweight, fast JSON parser for C#"",
        ""intendedUse"": ""For developers needing efficient JSON parsing in C# applications, particularly useful in high-performance scenarios"",
        ""tags"": [""C#"", ""JSON"", ""parsing"", ""high-performance"", ""library""],
        ""author"": ""Not specified in README"",
        ""url"": ""https://github.com/johndoe/fastjsonparser""
        }
        {
        ""description"": ""An open-source machine learning library for Python"",
        ""intendedUse"": ""Not specified in README"",
        ""tags"": [""Python"", ""machine-learning"", ""open-source""],
        ""author"": ""AI Research Team"",
        ""url"": ""https://github.com/airesearch/mlibrary""
        }

        Based solely on the provided README content and repository URL, what is the analysis of the repository? Do not include any information that is not explicitly stated in these sources.";
        private void PrintTokenUsage(IReadOnlyDictionary<string, object?>? metadata)
        {
            if (metadata != null && metadata.TryGetValue("Usage", out var usageObj) && usageObj is Dictionary<string, object> usage)
            {
                Console.WriteLine("Token usage:");
                PrintTokenDetail(usage, "PromptTokens", "Prompt tokens");
                PrintTokenDetail(usage, "CompletionTokens", "Completion tokens");
                PrintTokenDetail(usage, "TotalTokens", "Total tokens");
            }
            else
            {
                Console.WriteLine("Token usage information not available or not in the expected format.");
            }
        }

        private void PrintTokenDetail(Dictionary<string, object> usage, string key, string label)
        {
            if (usage.TryGetValue(key, out var value))
                Console.WriteLine($" {label}: {value}");
        }
    
        public async Task<RepoAnalysis> GetRepoReadmeDetailsAsync(Kernel kernel, string readmeText, string url)
        {
            var executionSettings = new OpenAIPromptExecutionSettings()
            {
                ResponseFormat = "json_object",
            };
            var arguments = new KernelArguments(executionSettings) { { "readmeContent", readmeText }, { "repoUrl", url } };

            try
            {
                var response = await kernel.InvokePromptAsync(_promptReadmeAnalysis, arguments);

                var analysisJson = response.GetValue<string>() ?? "{}";
                var repoResult = JsonSerializer.Deserialize<RepoAnalysis>(analysisJson);

                return repoResult ?? new RepoAnalysis();
                
                // PrintTokenUsage(response.Metadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new RepoAnalysis();
            }
        }

    }
    



    public class AgentHelper 
    {
        private Kernel _kernel;

        public AgentHelper(Kernel kernel)
        {
            this._kernel = kernel;
        }     

        public async Task<RepoAnalysis> GetRepoReadmeDetailsAsync(Kernel kernel, string readmeText, string url)
            {
                var executionSettings = new OpenAIPromptExecutionSettings()
                {
                    ResponseFormat = "json_object",
                };
                var arguments = new KernelArguments(executionSettings) { { "readmeContent", readmeText }, { "repoUrl", url } };

                try
                {
                    var response = await kernel.InvokePromptAsync(_promptReadmeAnalysis, arguments);

                    var analysisJson = response.GetValue<string>() ?? "{}";
                    var repoResult = JsonSerializer.Deserialize<RepoAnalysis>(analysisJson);

                    return repoResult ?? new RepoAnalysis();
                    
                    // PrintTokenUsage(response.Metadata);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return new RepoAnalysis();
                }
            }

    
    }