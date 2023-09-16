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
using DomainFeatures.HubDocuments.Services;

namespace DomainFeatures.Database
{
    public class HubDocumentsLoaderService
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        private readonly ImageAnalyzerService imageAnalyzerService;
        public HubDocumentsLoaderService(HubDocumentsSingleton hubDocumentsSingleton, ImageAnalyzerService imageAnalyzerService)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
            this.imageAnalyzerService = imageAnalyzerService;
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

                        var images = page.GetImages();
                        foreach (var image in images)
                        {
                            //hubDocument.Images.Add(await imageAnalyzerService.AnalyzeImage(image.RawBytes.ToArray()));
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
