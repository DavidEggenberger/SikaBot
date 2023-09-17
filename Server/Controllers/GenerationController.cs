using DomainFeatures.HubDocuments;
using DomainFeatures.HubDocuments.Services;
using IronPdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerationController : ControllerBase
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        private readonly TranslatorService translatorService;
        public GenerationController(HubDocumentsSingleton hubDocumentsSingleton, TranslatorService translatorService)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
            this.translatorService = translatorService;
        }

        [HttpPost]
        public async Task<FileStreamResult> GeneratePDF(
            [FromQuery] Guid? HubDocumentId,
            [FromQuery] List<GenerationConfiguration> configuration,
            [FromQuery] string language,
            [FromQuery] ImageOption imageOption
            )
        {
            var document = hubDocumentsSingleton.HubDocuments.FirstOrDefault(x => x.Id == HubDocumentId);

            var renderer = new ChromePdfRenderer();

            string rendering = $@"<h1 style=""font-color: red;"">Generated Sika PDF</h1>";

            if (configuration.Contains(GenerationConfiguration.Keywords))
            {
                if (string.IsNullOrEmpty(language) is false)
                {
                    var translation = await translatorService.Translate(string.Join(" ", document.KeyPhrases), "EN", language);
                    rendering += $@"<h4>Keywords: {translation}</h4>";
                }
                else
                {
                    rendering += $@"<h4>Keywords: {string.Join(", ", document.Keywords)}</h4>";
                }
            }

            if (configuration.Contains(GenerationConfiguration.Summarization))
            {
                if (string.IsNullOrEmpty(language) is false)
                {
                    var translation = await translatorService.Translate(string.Join(" ", document.Summarization.Where(s => s.Item1.ToLower() == "en").Select(s => s.Item2)), "EN", language);
                    rendering += $@"<h4>Summary: {translation}</h4>";
                }
                else
                {
                    rendering += $@"<h4>Summary: {string.Join(", ", document.Summarization.Where(s => s.Item1.ToLower() == "en").Select(s => s.Item2))}</h4>";
                }
            }

            if (imageOption is not null)
            {
                var image = document.Images
                    .Where(i => imageOption.ImageTags.Any(y => i.DetectionValues.Where(x => x.Confidence >= imageOption.MinConfidence).Select(s => s.Name).ToList().Contains(y)))
                    .First();

                rendering += $@"<img src=""{image.uri}"" />";
            }

            var pdf = await renderer.RenderHtmlAsPdfAsync(rendering);

            // Create a PDF from a HTML string using C#

            // Export to a file or Stream
            return new FileStreamResult(pdf.ToDocument().Stream, "application/pdf")
            {
                FileDownloadName = "SikaGenerated.pdf"
            };
        }
    }
    public class ImageOption
    {
        public List<string> ImageTags { get; set; }
        public double MinConfidence { get; set; } = 0.7;
        public int ImageCount { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GenerationConfiguration
    {
        Keywords = 0,
        Summarization = 1,
        Entities = 2,
        FullText = 3
    }
}
