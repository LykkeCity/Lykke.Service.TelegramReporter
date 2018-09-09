using System;
using Autofac;
using Common;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Services;
using Lykke.Service.TelegramReporter.Services.Instances;
using Lykke.Service.TelegramReporter.Settings;
using Lykke.SettingsReader;
using AzureStorage.Tables;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.TelegramReporter.AzureRepositories;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Balance;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.Balance;
using Lykke.Service.TelegramReporter.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.NettingEngine.Rabbit;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Services.WalletsRebalancer;
using Lykke.Service.TelegramReporter.Services.WalletsRebalancer;
using Lykke.Service.TelegramReporter.Services.WalletsRebalancer.Rabbit;
using Lykke.Service.MarketMakerReports.Client;

namespace Lykke.Service.TelegramReporter.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;            
        }

        protected override void Load(ContainerBuilder builder)
        {
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

            builder.RegisterInstance(new AssetsService(new Uri(_appSettings.CurrentValue.AssetsServiceClient.ServiceUrl)))
                .As<IAssetsService>()
                .SingleInstance();

            builder.RegisterAssetsClient(AssetServiceSettings.Create(
                new Uri(_appSettings.CurrentValue.AssetsServiceClient.ServiceUrl),
                _appSettings.CurrentValue.TelegramReporterService.AssetsCacheExpirationPeriod));

            builder.RegisterBalancesClient(_appSettings.CurrentValue.BalancesServiceClient.ServiceUrl);
            builder.RegisterMarketMakerReportsClient(_appSettings.CurrentValue.MarketMakerReportsServiceClient, null);

            builder.RegisterType<NettingEngineStateProvider>()
                .As<INettingEngineStateProvider>()
                .SingleInstance();

            builder.RegisterType<NettingEngineAuditProvider>()
                .As<INettingEngineAuditProvider>()
                .SingleInstance();

            builder.RegisterType<WalletsRebalancerProvider>()
                .As<IWalletsRebalancerProvider>()
                .SingleInstance();

            builder.RegisterType<BalanceWarningProvider>()
                .As<IBalanceWarningProvider>()
                .SingleInstance();

            builder.RegisterType<ExternalBalanceWarningProvider>()
                .As<IExternalBalanceWarningProvider>()
                .SingleInstance();

            builder.RegisterType<NettingEngineStateSubscriber>()
                .As<ITelegramSubscriber>();

            builder.RegisterType<NettingEngineAuditPublisher>()
                .As<INettingEngineAuditPublisher>();

            builder.RegisterType<WalletsRebalancerPublisher>()
                .As<IWalletsRebalancerPublisher>();

            builder.RegisterType<ChatPublisherService>()
                .As<IChatPublisherService>()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance();

            RegisterRepositories(builder);
            RegisterRabbitMqSubscribers(builder);
        }

        private void RegisterRepositories(ContainerBuilder builder)
        {
            builder.Register(container => new ChatPublisherSettingsRepository(
                AzureTableStorage<ChatPublisherSettingsEntity>
                    .Create(_appSettings.ConnectionString(x => x.TelegramReporterService.Db.DataConnString), "ChatPublisherSettings", container.Resolve<ILogFactory>())))                
                .As<IChatPublisherSettingsRepository>()
                .SingleInstance();

            builder.Register(container => new BalanceWarningRepository(
                AzureTableStorage<BalanceWarningEntity>
                    .Create(_appSettings.ConnectionString(x => x.TelegramReporterService.Db.DataConnString), "BalancesWarnings", container.Resolve<ILogFactory>())))
                .As<IBalanceWarningRepository>()
                .SingleInstance();

            builder.Register(container => new ExternalBalanceWarningRepository(
                AzureTableStorage<ExternalBalanceWarningEntity>
                    .Create(_appSettings.ConnectionString(x => x.TelegramReporterService.Db.DataConnString), "ExternalBalancesWarnings", container.Resolve<ILogFactory>())))
                .As<IExternalBalanceWarningRepository>()
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

            builder.RegisterType<WalletsRebalancerSubscriber>()
                .As<IStartable>()
                .As<IStopable>()
                .WithParameter("settings", _appSettings.CurrentValue.TelegramReporterService.WalletsRebalancerExchange)
                .AutoActivate()
                .SingleInstance();
        }
    }
}
