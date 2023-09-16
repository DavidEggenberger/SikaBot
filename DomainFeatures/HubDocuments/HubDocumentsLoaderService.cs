using DomainFeatures.HubDocuments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig;
using DomainFeatures.HubDocuments.Domain;

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
            foreach (var file in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Data\PDF\"), "*.pdf", SearchOption.AllDirectories))
            {
                HubDocument hubDocument = new HubDocument();
                using (PdfDocument document = PdfDocument.Open(file))
                {
                    string text = string.Empty;
                    foreach (Page page in document.GetPages())
                    {
                        if (text.Length < 2500)
                        {
                            text += $" {page.Text}";
                        }
                    }

                    hubDocument.Location = file;
                    hubDocument.Text = text;
                    hubDocument.Id = Guid.NewGuid();

                    hubDocumentsSingleton.HubDocuments.Add(hubDocument);
                }
            }
        }
    }
}
