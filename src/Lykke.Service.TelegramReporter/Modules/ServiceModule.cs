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
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.TelegramReporter.AzureRepositories;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Balance;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Core.Services.SpreadEngine;
using Lykke.Service.TelegramReporter.Services.Balance;
using Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.SpreadEngine;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.TelegramReporter.Services.NettingEngine.Rabbit;

namespace Lykke.Service.TelegramReporter.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly ILog _log;

        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AppSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

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

            builder.RegisterType<NettingEngineInstanceManager>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.NettingEngineServiceClient.Instances))
                .As<INettingEngineInstanceManager>()
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
                        
            builder.RegisterType<CmlStateProvider>()
                .As<ICmlStateProvider>()
                .SingleInstance();

            builder.RegisterInstance(new AssetsService(new Uri(_appSettings.CurrentValue.AssetsServiceClient.ServiceUrl)))
                .As<IAssetsService>()
                .SingleInstance();

            _services.RegisterAssetsClient(AssetServiceSettings
                .Create(new Uri(_appSettings.CurrentValue.AssetsServiceClient.ServiceUrl),
                    _appSettings.CurrentValue.TelegramReporterService.AssetsCacheExpirationPeriod), _log);

            // Register Balance client
            builder.RegisterBalancesClient(_appSettings.CurrentValue.BalancesServiceClient.ServiceUrl, _log);

            builder.RegisterRateCalculatorClient(_appSettings.CurrentValue.RateCalculatorServiceClient.ServiceUrl, _log);

            builder.RegisterType<SpreadEngineStateProvider>()
                .As<ISpreadEngineStateProvider>()
                .SingleInstance();

            builder.RegisterType<NettingEngineStateProvider>()
                .As<INettingEngineStateProvider>()
                .SingleInstance();

            builder.RegisterType<NettingEngineAuditProvider>()
                .As<INettingEngineAuditProvider>()
                .SingleInstance();

            builder.RegisterType<BalanceWarningProvider>()
                .As<IBalanceWarningProvider>()
                .SingleInstance();

            builder.RegisterType<CmlSummarySubscriber>()
                .As<ITelegramSubscriber>();

            builder.RegisterType<CmlStateSubscriber>()
                .As<ITelegramSubscriber>();

            builder.RegisterType<SpreadEngineStateSubscriber>()
                .As<ITelegramSubscriber>();

            builder.RegisterType<NettingEngineStateSubscriber>()
                .As<ITelegramSubscriber>();

            builder.RegisterType<NettingEngineAuditPublisher>()
                .As<INettingEngineAuditPublisher>();

            builder.RegisterType<ChatPublisherService>()
                .As<IChatPublisherService>()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance();

            RegisterRepositories(builder);
            RegisterRabbitMqSubscribers(builder);

            builder.Populate(_services);
        }

        private void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<ChatPublisherSettingsRepository>()
                .As<IChatPublisherSettingsRepository>()
                .WithParameter(TypedParameter.From(AzureTableStorage<ChatPublisherSettingsEntity>
                    .Create(_appSettings.ConnectionString(x => x.TelegramReporterService.Db.DataConnString), "ChatPublisherSettings", _log)))
                .SingleInstance();

            builder.RegisterType<BalanceWarningRepository>()
                .As<IBalanceWarningRepository>()
                .WithParameter(TypedParameter.From(AzureTableStorage<BalanceWarningEntity>
                    .Create(_appSettings.ConnectionString(x => x.TelegramReporterService.Db.DataConnString), "BalancesWarnings", _log)))
                .SingleInstance();
        }

        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            builder.RegisterType<NettingEngineAuditSubscriber>()
                .As<IStartable>()
                .As<IStopable>()
                .WithParameter("settings", _appSettings.CurrentValue.TelegramReporterService.NettingEngineAuditExchange)
                .AutoActivate()
                .SingleInstance();
        }
    }
}
