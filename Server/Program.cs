using Azure.Identity;
using DomainFeatures.Database;
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
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext) =>
                {
                    //hostingContext.AddAzureKeyVault(new Uri("https://dargebotenehand.vault.azure.net/"),
                            //new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = "249e3537-00dc-46b0-9eb2-6e422f9fa9b7" }));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
