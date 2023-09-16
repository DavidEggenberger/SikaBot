using DomainFeatures.HubDocuments;
using DomainFeatures.HubDocuments.Domain;
using DTOs.HubDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubDocumentsController : ControllerBase
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;

        public HubDocumentsController(HubDocumentsSingleton hubDocumentsSingleton)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }

        [HttpGet]
        public async Task<ActionResult<List<HubDocumentDTO>>> GetHubDocuments()
        {
            return Ok(hubDocumentsSingleton.HubDocuments.Select(s => s.ToDTO()).ToList());
        }

        [HttpPost]
        public async Task<ActionResult> SaveHubDocument(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.ContentType != "application/pdf")
            {
                return BadRequest("Please provide a PDF File");
            }

            return Ok();
        }
    }
}
