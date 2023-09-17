using DomainFeatures.HubDocuments;
using DTOs.HubDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        public ImagesController(HubDocumentsSingleton hubDocumentsSingleton) 
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }

        [HttpPost]
        public async Task<List<HubDocumentImageDTO>> GetImages(List<ImageSearch> imageSearches)
        {

            return hubDocumentsSingleton.HubDocuments.SelectMany(x => x.Images)
                .Where(x => imageSearches.All(p => x.DetectionValues.Any(x => x.Confidence >= p.MinConfidence && x.Name.ToLower() == p.Tag.ToLower())))
                .Select(x => x.ToDTO())
                .ToList();
        }
    }
    public class ImageSearch
    {
        public string Tag { get; set; }
        public double MinConfidence { get; set; } = 0.8;
    }
}
