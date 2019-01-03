using System;
using System.Collections.Generic;
using System.Linq;
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
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.NettingEngine.Rabbit;
using Lykke.Common.Log;
using Lykke.HttpClientGenerator.Infrastructure;
using Lykke.Service.MarketMakerArbitrageDetector.Client;
using Lykke.Service.TelegramReporter.Core.Services.WalletsRebalancer;
using Lykke.Service.TelegramReporter.Services.WalletsRebalancer;
using Lykke.Service.TelegramReporter.Services.WalletsRebalancer.Rabbit;
using Lykke.Service.MarketMakerReports.Client;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;
using Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine;
using Lykke.Service.TelegramReporter.Core.Services.MarketMakerArbitrages;
using Lykke.Service.TelegramReporter.Services.Channelv2;
using Lykke.Service.TelegramReporter.Services.CryptoIndex.InstancesSettings;
using Lykke.Service.TelegramReporter.Services.LiquidityEngine;
using Lykke.Service.TelegramReporter.Services.MarketMakerArbitrages;
using CryptoIndexClientSettings = Lykke.Service.TelegramReporter.Services.CryptoIndex.InstancesSettings.CryptoIndexClientSettings;

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

            builder.RegisterType<ChannelManager>()
                .As<IChannelManager>()
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

            RegiaterFiatMarketMakerReportsClient(builder, _appSettings.CurrentValue.FiatMarketMakerReportsServiceClient);

            builder.RegisterMarketMakerArbitrageDetectorClient(new MarketMakerArbitrageDetectorServiceClientSettings
                { ServiceUrl = _appSettings.CurrentValue.MarketMakerArbitrageDetectorServiceClient.ServiceUrl }, null);

            builder.RegisterInstance(
                    new LiquidityEngineUrlSettings(_appSettings.CurrentValue.LiquidityEngineServiceClient.Instances.Select(e => e.ServiceUrl).ToArray()))
                .SingleInstance();

            var cryptoIndexInstances = new List<CryptoIndexClientSettings>();
            foreach (var cics in _appSettings.CurrentValue.CryptoIndexServiceClient.Instances)
                cryptoIndexInstances.Add(new CryptoIndexClientSettings { DisplayName = cics.DisplayName, ServiceUrl = cics.ServiceUrl });
            builder.RegisterInstance(
                    new CryptoIndexInstancesSettings
                    {
                        Instances = cryptoIndexInstances.ToArray()
                    })
                .SingleInstance();

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

            builder.RegisterType<MarketMakerArbitragesWarningProvider>()
                .As<IMarketMakerArbitragesWarningProvider>()
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

            builder.RegisterType<ChatPublisherStateService>()
                .As<IChatPublisherStateService>()                
                .SingleInstance();

            RegisterRepositories(builder);
            RegisterRabbitMqSubscribers(builder);
        }

        private void RegiaterFiatMarketMakerReportsClient(ContainerBuilder builder,
            MarketMakerReportsServiceClientSettings settings)
        {
            var clientBuilder = HttpClientGenerator.HttpClientGenerator.BuildForUrl(settings.ServiceUrl)
                .WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper())
                .WithoutRetries();

            var client = new MarketMakerReportsFiatClient(new MarketMakerReportsClient(clientBuilder.Create()), settings.ServiceUrl);

            builder.RegisterInstance(client)
                .As<IMarketMakerReportsFiatClient>()
                .SingleInstance();
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

            builder.Register(container => new ChannelRepository(
                    AzureTableStorage<ReportChannelEntity>
                        .Create(_appSettings.ConnectionString(x => x.TelegramReporterService.Db.DataConnString), "ReportChannelSettings", container.Resolve<ILogFactory>())))
                .As<IChannelRepository>()
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
