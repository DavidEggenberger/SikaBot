using DomainFeatures.HubDocuments;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Server.BackgroundJobs
{
    public class ReindexingJob : BackgroundService
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        public ReindexingJob(HubDocumentsSingleton hubDocumentsSingleton)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(stoppingToken.IsCancellationRequested is false)
            {
                


                await Task.Delay(5000);
            }
        }
    }
}
