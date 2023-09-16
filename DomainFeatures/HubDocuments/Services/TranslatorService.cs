using DomainFeatures.HubDocuments.Domain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments.Services
{
    public class TranslatorService
    {
        private readonly IConfiguration configuration;
        public TranslatorService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task TranslateHubDocumentsAsync(List<HubDocument> hubDocuments, List<string> translationIds = null)
        {
            var languages = new List<string> { "ar", "DE", "it" };

            if (translationIds != null)
            {
                languages.AddRange(translationIds);
            }

            foreach (var document in hubDocuments)
            {
                foreach (var item in languages)
                {
                    document.Summarization.Add(new (item, await Translate(document.Summarization.FirstOrDefault(s => s.Item1 == "en").Item2, "en", item)));
                }
            }
        }

        public async Task<string> Translate(string textToTranslate, string fromLanguage, string toLanguage)
        {
            if (toLanguage == "ch-DE")
            {
                toLanguage = "DE";
            }

            string route = $"/translate?api-version=3.0&from={fromLanguage}&to={toLanguage}";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonSerializer.Serialize(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri("https://api.cognitive.microsofttranslator.com/" + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", configuration["SikaTranslator"]);
                request.Headers.Add("Ocp-Apim-Subscription-Region", "switzerlandnorth");

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string s = await response.Content.ReadAsStringAsync();
                List<Root> root = JsonSerializer.Deserialize<List<Root>>(s, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true });
                return String.Join(" ", root.SelectMany(s => s.Translations).Select(s => s.Text));
            }
        }
    }
    public class Translation
    {
        public string Text { get; set; }
        public string To { get; set; }
    }

    public class Root
    {
        public List<Translation> Translations { get; set; }
    }
}
