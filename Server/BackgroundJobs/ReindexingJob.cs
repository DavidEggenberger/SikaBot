using DomainFeatures.HubDocuments;
using DomainFeatures.HubDocuments.Domain;
using DomainFeatures.HubDocuments.Services;
using DTOs.Chat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System;
using System.IO;
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
                using (var scope = services.CreateScope())
                {
                    var reportStore =
                        scope.ServiceProvider
                            .GetRequiredService<ReportStore>();

                    var hubDocuments = scope.ServiceProvider
                            .GetRequiredService<HubDocumentsSingleton>();

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage())
                    {
                        var sheet = package.Workbook.Worksheets.Add("Report_Sheet");

                        sheet.Cells[1, 1].Value = "HubDocument Id";
                        sheet.Cells[1, 2].Value = "Link";
                        sheet.Cells[1, 3].Value = "Generated Derivations Count";
                        sheet.Cells[1, 4].Value = "Retrieval Count";

                        int row = 2;
                        foreach (var document in hubDocuments.HubDocuments)
                        {
                            sheet.Cells[row, 1].Value = document.Id;
                            sheet.Cells[row, 2].Value = document.Uri;
                            sheet.Cells[row, 3].Value = document.Generations;
                            sheet.Cells[row, 4].Value = document.Retrievals;

                            row++;
                        }

                        reportStore.Init = true;
                        reportStore.excel = new MemoryStream(await package.GetAsByteArrayAsync());
                    }

                    await Task.Delay(5000);
                }
            }
        }
    }
    public class ReportStore
    {
        public bool Init { get; set; }
        public MemoryStream excel = new MemoryStream();
    }
}
