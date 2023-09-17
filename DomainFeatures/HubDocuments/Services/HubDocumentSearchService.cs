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
            var foundResult = new List<HubDocumentDTO>();
            if (supportedLanguages?.Any() == true)
            {
                if(hubDocumentsSingleton.HubDocuments
                    .Where(s => s.Summarization.Any(x => supportedLanguages.Contains(x.Item1) && tags.Any(t => x.Item2.Contains(t))))
                    ?.Any() == true)
                {
                    foundResult.AddRange(hubDocumentsSingleton.HubDocuments
                        .Where(s => s.Summarization.Any(x => supportedLanguages.Contains(x.Item1) && tags.Any(t => x.Item2.Contains(t))))
                        .Select(x => x.ToDTO()));
                }
            }

            if (tags?.Any() == true)
            {
                if (hubDocumentsSingleton.HubDocuments
                    .Where(s => s.Summarization.Any(x => tags.Any(t => x.Item2.Contains(t))))
                    ?.Any() == true)
                {
                    foundResult.AddRange(hubDocumentsSingleton.HubDocuments
                        .Where(s => s.Summarization.Any(x => tags.Any(t => x.Item2.Contains(t))))
                        .Select(x => x.ToDTO()));
                }
            }

            if (recognizedEntities?.Any() == true)
            {
                if (hubDocumentsSingleton.HubDocuments
                    .Where(s => s.Entities.Any(x => tags.Any(t => x.ToLower() == t.ToLower())))
                    ?.Any() == true)
                {
                    foundResult.AddRange(hubDocumentsSingleton.HubDocuments
                    .Where(s => s.Entities.Any(x => tags.Any(t => x.ToLower() == t.ToLower())))
                        ?.Select(x => x.ToDTO()));
                }
            }

            if (excludeImages is false)
            {

            }

            if (foundResult.Any() == true) 
            {
                return foundResult;
            }

            return  hubDocumentsSingleton.HubDocuments
                .Select(x => x.ToDTO())
                .ToList();
        }
    }
}
