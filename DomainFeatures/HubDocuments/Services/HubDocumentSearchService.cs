using DomainFeatures.HubDocuments.Domain;
using DTOs.HubDocuments;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments.Services
{
    public class HubDocumentSearchService
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        public HubDocumentSearchService(HubDocumentsSingleton hubDocumentsSingleton)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }


        public List<HubDocumentDTO> SearchHubDocuments(
            IList<string> tags, 
            IList<string> supportedLanguages, 
            IList<string> recognizedEntities,
            bool? excludeImages)
        {



            return null;
        }
    }
}
