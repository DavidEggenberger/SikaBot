using DomainFeatures;
using DomainFeatures.BlobClient;
using DomainFeatures.Chats;
using DomainFeatures.Database;
using DomainFeatures.HubDocuments;
using DomainFeatures.HubDocuments.Services;
using DomainFeatures.OpenAi;
using DomainFeatures.QuestionAnswering;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Server.BackgroundJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IronPdf.License.LicenseKey = Configuration["IronPDF"];

            services.AddControllers(options =>
            {

            }).AddJsonOptions(json =>
            {
                json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddSignalR();

            services.AddScoped<HubDocumentSearchService>();
            services.AddSingleton<BlobStorageConstants>();
            services.AddScoped<BlobClientService>();
            services.AddScoped<ImageAnalyzerService>();
            services.AddScoped<ChatBotService>();
            services.AddScoped<QuestionAnswererService>();
            services.AddScoped<TranslatorService>();
            services.AddScoped<SummarizerService>();
            services.AddSingleton<ChatPersistence>();
            services.AddScoped<HubDocumentsLoaderService>();
            services.AddSingleton<HubDocumentsSingleton>();
            services.AddScoped<OpenAIService>();
            services.AddHostedService<ReindexingJob>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sika Bot", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.InjectStylesheet("/swaggerStyles.css");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sika Hub v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/NotificationHub");
            });
        }
    }
}
