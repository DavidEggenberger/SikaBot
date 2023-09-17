using Azure;
using DomainFeatures.HubDocuments.Domain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Azure.AI.TextAnalytics;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments.Services
{
    public class SummarizerService
    {
        private readonly IConfiguration configuration;
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        public SummarizerService(IConfiguration configuration, HubDocumentsSingleton hubDocumentsSingleton)
        {
            this.configuration = configuration;
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }

        public async Task SummarizeHubDocuments()
        {
            string languageKey = configuration["SikaAI"];
            string languageEndpoint = "https://sikaailanguage.cognitiveservices.azure.com/";

            AzureKeyCredential credentials = new AzureKeyCredential(languageKey);
            Uri endpoint = new Uri(languageEndpoint);

            var client = new TextAnalyticsClient(endpoint, credentials);

            var batchInput = hubDocumentsSingleton.HubDocuments.Select(h => new TextDocumentInput(h.Id.ToString(), h.Text));

            TextAnalyticsActions actions = new TextAnalyticsActions()
            {
                RecognizeEntitiesActions = new List<RecognizeEntitiesAction>() { new RecognizeEntitiesAction() { ActionName = "RecognieEntity"} },
                ExtractiveSummarizeActions = new List<ExtractiveSummarizeAction>() { new ExtractiveSummarizeAction() { ActionName = "ExtraceSummarize"} },
                ExtractKeyPhrasesActions = new List<ExtractKeyPhrasesAction>() { new ExtractKeyPhrasesAction() { ActionName = "ExtractKeyPhrasesSample" } },
            };

            foreach (var chunkedDocuments in batchInput.Chunk(10))
            {
                // Start analysis process.
                AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(chunkedDocuments, actions);

                await operation.WaitForCompletionAsync();

                await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
                {
                    IReadOnlyCollection<ExtractKeyPhrasesActionResult> keyPhraseResults = documentsInPage.ExtractKeyPhrasesResults;
                    IReadOnlyCollection<ExtractiveSummarizeActionResult> summarizeResults = documentsInPage.ExtractiveSummarizeResults;
                    IReadOnlyCollection<RecognizeEntitiesActionResult> entitiesResults = documentsInPage.RecognizeEntitiesResults;

                    var recognizeEntitiesResults = entitiesResults.SelectMany(s => s.DocumentsResults).ToList();
                    var summarizationResults = summarizeResults.SelectMany(s => s.DocumentsResults).ToList();
                    var keyPhrasesResults = keyPhraseResults.SelectMany(s => s.DocumentsResults).ToList();

                    foreach (var keyPhraseResult in keyPhrasesResults)
                    {
                        var hubDocument = hubDocumentsSingleton.HubDocuments.First(h => h.Id == new Guid(keyPhraseResult.Id));
                        hubDocument.KeyPhrases = keyPhraseResult.KeyPhrases.ToList();
                    }

                    foreach (var summarizeResult in summarizationResults)
                    {
                        var hubDocument = hubDocumentsSingleton.HubDocuments.First(h => h.Id == new Guid(summarizeResult.Id));
                        hubDocument.Summarization = new List<(string, string)> { new("en", string.Join(", ", summarizeResult.Sentences.OrderByDescending(s => s.RankScore).Select(s => s.Text).ToList())) };
                    }

                    foreach (var recognizeEntities in recognizeEntitiesResults)
                    {
                        var hubDocument = hubDocumentsSingleton.HubDocuments.First(h => h.Id == new Guid(recognizeEntities.Id));
                        hubDocument.Entities = recognizeEntities.Entities.Select(s => s.Text).ToList();
                    }
                }
            }          
        }

        public async Task SummarizeHubDocumentAsync(HubDocument document)
        {
            string languageKey = configuration["SikaAI"];
            string languageEndpoint = "https://sikaailanguage.cognitiveservices.azure.com/";

            AzureKeyCredential credentials = new AzureKeyCredential(languageKey);
            Uri endpoint = new Uri(languageEndpoint);

            var client = new TextAnalyticsClient(endpoint, credentials);

            TextAnalyticsActions actions = new TextAnalyticsActions()
            {
                RecognizeEntitiesActions = new List<RecognizeEntitiesAction>() { new RecognizeEntitiesAction() { ActionName = "RecognieEntity" } },
                ExtractiveSummarizeActions = new List<ExtractiveSummarizeAction>() { new ExtractiveSummarizeAction() { ActionName = "ExtraceSummarize" } },
                ExtractKeyPhrasesActions = new List<ExtractKeyPhrasesAction>() { new ExtractKeyPhrasesAction() { ActionName = "ExtractKeyPhrasesSample" } },
            };

            var documentInput = new TextDocumentInput(document.Id.ToString(), document.Text);

            // Start analysis process.
            AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(new List<TextDocumentInput> { documentInput }, actions);

            await operation.WaitForCompletionAsync();

            await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
            {
                IReadOnlyCollection<ExtractKeyPhrasesActionResult> keyPhraseResults = documentsInPage.ExtractKeyPhrasesResults;
                IReadOnlyCollection<ExtractiveSummarizeActionResult> summarizeResults = documentsInPage.ExtractiveSummarizeResults;
                IReadOnlyCollection<RecognizeEntitiesActionResult> entitiesResults = documentsInPage.RecognizeEntitiesResults;

                var recognizeEntitiesResults = entitiesResults.SelectMany(s => s.DocumentsResults).ToList();
                var summarizationResults = summarizeResults.SelectMany(s => s.DocumentsResults).ToList();
                var keyPhrasesResults = keyPhraseResults.SelectMany(s => s.DocumentsResults).ToList();

                foreach (var keyPhraseResult in keyPhrasesResults)
                {
                    var hubDocument = hubDocumentsSingleton.HubDocuments.First(h => h.Id == new Guid(keyPhraseResult.Id));
                    hubDocument.KeyPhrases = keyPhraseResult.KeyPhrases.Take(10).ToList();
                }

                foreach (var summarizeResult in summarizationResults)
                {
                    var hubDocument = hubDocumentsSingleton.HubDocuments.First(h => h.Id == new Guid(summarizeResult.Id));
                    hubDocument.Summarization = new List<(string, string)> { new("en", string.Join(", ", summarizeResult.Sentences.OrderByDescending(s => s.RankScore).Select(s => s.Text).ToList())) };
                }

                foreach (var recognizeEntities in recognizeEntitiesResults)
                {
                    var hubDocument = hubDocumentsSingleton.HubDocuments.First(h => h.Id == new Guid(recognizeEntities.Id));
                    hubDocument.Entities = recognizeEntities.Entities.Select(s => s.Text).ToList();
                }
            }
        }
        
    }
}
