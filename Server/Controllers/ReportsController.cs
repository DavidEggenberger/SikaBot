using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.BackgroundJobs;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportStore reportStore;
        public ReportsController(ReportStore reportStore)
        {
            this.reportStore = reportStore;
        }

        [HttpGet]
        public async Task<FileContentResult> GetReport()
        {
            if (reportStore.Init is false)
            {
                return null;
            }

            var file = new FileContentResult(reportStore.excel.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            file.FileDownloadName = "SikaReport.xlsx";

            return file;
        }
    }
}
