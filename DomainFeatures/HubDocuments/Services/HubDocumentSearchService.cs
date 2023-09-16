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
            if (supportedLanguages?.Any() == true)
            {
                return hubDocumentsSingleton.HubDocuments
                    .Where(s => s.Summarization.Any(x => supportedLanguages.Contains(x.Item1) && tags.Any(t => x.Item2.Contains(t))))
                    .Select(x => x.ToDTO())
                    .ToList();
            }

            if (tags?.Any() == true)
            {
                return hubDocumentsSingleton.HubDocuments
                    .Where(s => s.Summarization.Any(x => tags.Any(t => x.Item2.Contains(t))))
                    .Select(x => x.ToDTO())
                    .ToList();
            }

            if (recognizedEntities?.Any() == true)
            {
                return hubDocumentsSingleton.HubDocuments
                    .Where(s => s.Entities.Any(x => tags.Any(t => x.ToLower() == t.ToLower())))
                    .Select(x => x.ToDTO())
                    .ToList();
            }


            return hubDocumentsSingleton.HubDocuments
                .Select(x => x.ToDTO())
                .ToList();
        }
    }
}
