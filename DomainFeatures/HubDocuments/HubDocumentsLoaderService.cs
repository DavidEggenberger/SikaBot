using DomainFeatures.HubDocuments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.Database
{
    public class HubDocumentsLoaderService
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        public HubDocumentsLoaderService(HubDocumentsSingleton hubDocumentsSingleton)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }
        public async Task LoadHubDocumentsAsnyc()
        {
            string s = typeof(HubDocumentsLoaderService).Assembly.Location;
            var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(typeof(HubDocumentsLoaderService).Assembly.Location), @"Data\PDF\Brochures"));
            foreach (var file in files)
            {

            }

        }
    }
}
