using Azure;
using Azure.AI.Language.QuestionAnswering;
using DomainFeatures.HubDocuments;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.QuestionAnswering
{
    public class QuestionAnswererService
    {
        private readonly IConfiguration configuration;
        private readonly HubDocumentsSingleton hubDocumentsSingleton;

        public QuestionAnswererService(IConfiguration configuration, HubDocumentsSingleton hubDocumentsSingleton) 
        { 
            this.configuration = configuration;
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }

        public async Task<string> AnswerQuestion(string question)
        {
            string languageKey = configuration["SikaAI"];
            string languageEndpoint = "https://sikaailanguage.cognitiveservices.azure.com/";

            AzureKeyCredential credential = new AzureKeyCredential(languageKey);
            Uri endpoint = new Uri(languageEndpoint);

            string projectName = "SikaAI";
            string deploymentName = "production";

            QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);
            QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

            return "";
        }

        public async Task UploadDocument()
        {
            string languageKey = configuration["SikaAI"];
            string languageEndpoint = "https://sikaailanguage.cognitiveservices.azure.com/";

            AzureKeyCredential credential = new AzureKeyCredential(languageKey);
            Uri endpoint = new Uri(languageEndpoint);

            string projectName = "SikaAI";
            string deploymentName = "production";

            QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);
            QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

            string str = string.Join(", ", hubDocumentsSingleton.HubDocuments.SelectMany(s => s.Summarization.First().Item2));
            var b = await client.GetAnswersFromTextAsync("What does Sika do?", new List<string> { str });
        }
    }
}
