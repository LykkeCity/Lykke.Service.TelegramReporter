using Autofac;
using Common;
using Lykke.Sdk;
using Lykke.Service.CrossMarketLiquidity.Client;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Services;
using Lykke.Service.TelegramReporter.Services.Instances;
using Lykke.Service.TelegramReporter.Settings;
using Lykke.SettingsReader;
using System.Linq;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity;

namespace Lykke.Service.TelegramReporter.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<AppSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<TelegramService>()
                .WithParameter("settings", _appSettings.CurrentValue.TelegramReporterService.Telegram)
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterInstance<ICrossMarketLiquidityClient[]>(
                _appSettings.CurrentValue.CrossMarketLiquidityServiceClient.Instances
                    .Select(i => new CrossMarketLiquidityClient(i.ServiceUrl, _log)).ToArray());
            builder.RegisterType<CrossMarketLiquidityInstanceManager>()
                .As<ICrossMarketLiquidityInstanceManager>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<CMLSummaryProvider>()
                .As<ICMLSummaryProvider>()
                .SingleInstance();
        }
    }
}
