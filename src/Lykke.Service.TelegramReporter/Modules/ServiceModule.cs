using System;
using Autofac;
using Common;
using Lykke.Service.CrossMarketLiquidity.Client;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Services;
using Lykke.Service.TelegramReporter.Services.Instances;
using Lykke.Service.TelegramReporter.Settings;
using Lykke.SettingsReader;
using System.Linq;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Core.Services.SpreadEngine;
using Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Services.SpreadEngine;

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

            builder.RegisterInstance<ICrossMarketLiquidityClient[]>(
                _appSettings.CurrentValue.CrossMarketLiquidityServiceClient.Instances
                    .Select(i => new CrossMarketLiquidityClient(i.ServiceUrl, _log)).ToArray());
            builder.RegisterType<CrossMarketLiquidityInstanceManager>()
                .As<ICrossMarketLiquidityInstanceManager>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<SpreadEngineInstanceManager>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.SpreadEngineServiceClient.Instances))
                .As<ISpreadEngineInstanceManager>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<TelegramService>()
                .As<ITelegramSender>()
                .WithParameter("settings", _appSettings.CurrentValue.TelegramReporterService.Telegram)
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<CmlSummaryProvider>()
                .As<ICmlSummaryProvider>()
                .SingleInstance();

            foreach (var publisherSettings in _appSettings.CurrentValue.TelegramReporterService.CmlPublisher)
            {
                builder.RegisterType<CmlSummarySubscriber>()
                    .WithParameter("publisherSettings", publisherSettings)
                    .As<ITelegramSubscriber>();
            }            

            builder.RegisterType<CmlStateProvider>()
                .As<ICmlStateProvider>()
                .SingleInstance();

            foreach (var publisherSettings in _appSettings.CurrentValue.TelegramReporterService.CmlPublisher)
            {
                builder.RegisterType<CmlStateSubscriber>()
                    .WithParameter("publisherSettings", publisherSettings)
                    .As<ITelegramSubscriber>();
            }

            builder.RegisterInstance(new AssetsService(new Uri(_appSettings.CurrentValue.AssetsServiceClient.ServiceUrl)))
                .As<IAssetsService>()
                .SingleInstance();

            builder.RegisterType<SpreadEngineStateProvider>()
                .As<ISpreadEngineStateProvider>()
                .SingleInstance();

            foreach (var publisherSettings in _appSettings.CurrentValue.TelegramReporterService.SpreadEnginePublisher)
            {
                builder.RegisterType<SpreadEngineStateSubscriber>()
                    .WithParameter("publisherSettings", publisherSettings)
                    .As<ITelegramSubscriber>();
            }

            foreach (var publisherSettings in _appSettings.CurrentValue.TelegramReporterService.CmlPublisher)
            {
                builder.RegisterType<CmlPublisher>()
                    .WithParameter("publisherSettings", publisherSettings)
                    .As<IStartable>()
                    .As<IStopable>()
                    .AutoActivate()
                    .SingleInstance();
            }

            foreach (var publisherSettings in _appSettings.CurrentValue.TelegramReporterService.SpreadEnginePublisher)
            {
                builder.RegisterType<SpreadEnginePublisher>()
                    .WithParameter("publisherSettings", publisherSettings)
                    .As<IStartable>()
                    .As<IStopable>()
                    .AutoActivate()
                    .SingleInstance();
            }
        }
    }
}
