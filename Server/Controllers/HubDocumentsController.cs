using DomainFeatures.HubDocuments;
using DomainFeatures.HubDocuments.Domain;
using DomainFeatures.HubDocuments.Services;
using DTOs.HubDocuments;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubDocumentsController : ControllerBase
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly SummarizerService summarizerService;
        private readonly TranslatorService translatorService;
        private readonly ImageAnalyzerService imageAnalyzerService;
        public HubDocumentsController(TranslatorService translatorService, ImageAnalyzerService imageAnalyzerService, HubDocumentsSingleton hubDocumentsSingleton, IWebHostEnvironment webHostEnvironment, SummarizerService summarizerService)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
            this.webHostEnvironment = webHostEnvironment;
            this.summarizerService = summarizerService;
            this.translatorService = translatorService;
            this.imageAnalyzerService = imageAnalyzerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<HubDocumentDTO>>> SearchHubDocuments(
            [FromQuery] IList<string> tags,
            [FromQuery] IList<string> supportedLanguages,
            [FromQuery] IList<string> recognizedEntities
            )
        {
            return Ok(hubDocumentsSingleton.HubDocuments.Select(s => s.ToDTO()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HubDocumentDTO>> GetHubDocument(Guid id)
        {
            return Ok(hubDocumentsSingleton.HubDocuments.FirstOrDefault(s => s.Id == id).ToDTO());
        }

        [HttpPost]
        public async Task<ActionResult<HubDocumentDTO>> SaveHubDocument(IFormFile pdfFile, [FromQuery] List<string> languages)
        {
            if (pdfFile == null || pdfFile.ContentType != "application/pdf")
            {
                return BadRequest("Please provide a PDF File");
            }

            var id = Guid.NewGuid();
            var generatedFileName = $@"{webHostEnvironment.WebRootPath}/{id}.pdf";

            using (var fileStream = pdfFile.OpenReadStream())
            {
                using (var destinationStream = new FileStream(generatedFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    await fileStream.CopyToAsync(destinationStream);
                    await fileStream.FlushAsync();
                }
            };

            var hubDocument = new HubDocument() { Id = Guid.NewGuid() };
            using (PdfDocument document = PdfDocument.Open(generatedFileName))
            {
                string text = string.Empty;
                foreach (Page page in document.GetPages())
                {
                    if (text.Length < 2500)
                    {
                        text += $" {page.Text}";
                    }
                    var images = page.GetImages();
                    foreach (var image in images)
                    {
                        hubDocument.Images.Add(await imageAnalyzerService.AnalyzeImage(image.RawBytes.ToArray()));
                    }
                }

                hubDocument.Text = text;
            }

            if (System.IO.File.Exists(generatedFileName))
            {
                System.IO.File.Delete(generatedFileName);
            }

            hubDocumentsSingleton.AddHubDocument(hubDocument);

            await summarizerService.SummarizeHubDocumentAsync(hubDocument);
            await translatorService.TranslateHubDocumentsAsync(new List<HubDocument> { hubDocument });

            return Ok(hubDocument.ToDTO());
        }
    }
}
