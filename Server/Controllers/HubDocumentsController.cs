using DomainFeatures.HubDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> Get()
        {
            return Ok(hubDocumentsSingleton.HubDocuments.Count);
        }
    }
}
