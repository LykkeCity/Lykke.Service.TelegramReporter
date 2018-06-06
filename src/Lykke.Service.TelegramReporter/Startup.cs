using System;
using Autofac;
using Lykke.Sdk;
using Lykke.Service.TelegramReporter.Settings;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.TelegramReporter
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                //options.ApiVersion = "v1";
                options.ApiTitle = "TelegramReporter API";
                options.LogsConnectionStringFactory = ctx => ctx.ConnectionString(x => x.TelegramReporterService.Db.LogsConnString);
                options.LogsTableName = "TelegramReporterLog";
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();
        }
    }
}
