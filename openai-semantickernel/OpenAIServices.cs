using Microsoft.SemanticKernel;
using System;
using System.IO;
using System.Threading.Tasks;

namespace openai_semantickernel
{
   
    public class OpenAIServices
    {
        private static string _openAIEndpoint = Env.Var("Azure_OPEN_AI_EndPoint");
        private static string _openAIKey = Env.Var("Azure_OPEN_AI_Secret_Key");
        private static string _openAIDeploymentName = Env.Var("AzureOpen_AI_Deployment_Name");

        public async Task<string> getOpenAIResponse(string searchQuestion)
        {
            var builder = new KernelBuilder();

            builder.WithAzureChatCompletionService(
            _openAIDeploymentName,                // Azure OpenAI Deployment Name
            _openAIEndpoint, // Azure OpenAI Endpoint
            _openAIKey);      // Azure OpenAI Key

            var kernel = builder.Build();

            var prompt = @"{{$input}}return the output based on prompt context.";

            var summarize = kernel.CreateSemanticFunction(prompt, maxTokens: 100, temperature: 0, topP: 0.5);

            var summary = await summarize.InvokeAsync(searchQuestion).ConfigureAwait(false);

            return summary.Result;
        }

        public async Task<string> getOpenAIResponseUsingSkill(string searchQuestion, string skill)
        {

            var builder = new KernelBuilder();

            builder.WithAzureChatCompletionService(
             _openAIDeploymentName,                // Azure OpenAI Deployment Name
            _openAIEndpoint, // Azure OpenAI Endpoint
            _openAIKey);      // Azure OpenAI Key

            var kernel = builder.Build();

            var skillsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Skills");

            var skillFunctions = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "SkillRandom");

            var result = await skillFunctions[skill].InvokeAsync(searchQuestion);

            return result.Result;
        }

    }
}
    
