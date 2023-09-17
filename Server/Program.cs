using Azure.Identity;
using DomainFeatures.BlobClient;
using DomainFeatures.Database;
using DomainFeatures.HubDocuments.Services;
using DomainFeatures.QuestionAnswering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope serviceScope = host.Services.CreateScope())
            {
                var hubsDocumentLoader = serviceScope.ServiceProvider.GetRequiredService<HubDocumentsLoaderService>();
                await hubsDocumentLoader.LoadHubDocumentsAsnyc();

                var summarizerService = serviceScope.ServiceProvider.GetRequiredService<SummarizerService>();
                await summarizerService.SummarizeHubDocuments();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext) =>
                {
                    hostingContext.AddAzureKeyVault(new Uri("https://sikavault.vault.azure.net/"),
                            new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = "fa68786c-a92c-47c9-8db2-9ae809ca847e" }));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
