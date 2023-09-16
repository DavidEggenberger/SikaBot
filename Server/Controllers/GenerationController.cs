using DomainFeatures.HubDocuments;
using IronPdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerationController : ControllerBase
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        public GenerationController(HubDocumentsSingleton hubDocumentsSingleton)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }

        [HttpPost]
        public async Task<FileStreamResult> GeneratePDF(
            [FromQuery] Guid? HubDocumentId,
            [FromQuery] IList<GenerationConfiguration> configuration,
            [FromQuery] IList<string> languageOptions,
            [FromQuery] 
            )
        {
            var document = hubDocumentsSingleton.HubDocuments.FirstOrDefault(x => x.Id == HubDocumentId);



            var renderer = new ChromePdfRenderer();

            var yellow = "#F5B325";
            var red = "#D8282F";

            renderer.RenderingOptions.CustomCssUrl = "./styles.css";

            // Create a PDF from a HTML string using C#
            var pdf = await renderer.RenderHtmlAsPdfAsync($@"<h1 class=""red"">Generated Sika PDF</h1>");

            // Export to a file or Stream
            return new FileStreamResult(pdf.ToDocument().Stream, "application/pdf")
            {
                FileDownloadName = "SikaGenerated.pdf"
            };
        }
    }
    public class ImageOption
    {

    }
    public enum GenerationConfiguration
    {
        Keywords,
        Summarization,
        Entities,
        FullText
    }
}
