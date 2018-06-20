using Lykke.Sdk;
using Lykke.Service.TelegramReporter.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.Service.TelegramReporter
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "LykkeService API";
                options.Logs = ("LykkeServiceLog", ctx => ctx.TelegramReporterService.Db.LogsConnString);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();
        }
    }
}
