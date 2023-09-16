using DomainFeatures.HubDocuments.Domain;
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


        [HttpGet]
        public List<HubDocument> SearchHubDocumentsAsync(List<string> keywoards)
        {
            return hubDocumentsSingleton.HubDocuments;
        }
    }
}
