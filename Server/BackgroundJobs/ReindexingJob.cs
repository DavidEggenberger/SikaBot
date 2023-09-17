using DomainFeatures.HubDocuments;
using DomainFeatures.HubDocuments.Domain;
using DomainFeatures.HubDocuments.Services;
using DTOs.Chat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace Server.BackgroundJobs
{
    public class ReindexingJob : BackgroundService
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        private readonly IServiceProvider services;

        public ReindexingJob(HubDocumentsSingleton hubDocumentsSingleton, IServiceProvider services)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
            this.services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested is false)
            {
                //var HubDocument = hubDocumentsSingleton.HubDocuments.FirstOrDefault(x => x.PictureExtracted is false);

                //if (HubDocument is not null)
                //{
                //    using (var scope = services.CreateScope())
                //    {
                //        var imageAnalyzerService = scope.ServiceProvider.GetRequiredService<ImageAnalyzerService>();


                //    }


                //}

                await Task.Delay(5000);
            }
        }
    }
}
