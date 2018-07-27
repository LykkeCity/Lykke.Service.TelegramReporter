using System;
using System.Collections.Generic;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Instances;
using JetBrains.Annotations;
using Lykke.Service.NettingEngine.Client.Api;
using Lykke.Service.NettingEngine.Client.Models.ServiceInfo;

namespace Lykke.Service.TelegramReporter.Services.Instances
{
    [UsedImplicitly]
    public class NettingEngineInstanceManager : INettingEngineInstanceManager
    {
        private readonly object _sync = new object();
        private readonly string[] _instances;
        private readonly ILog _log;
        private List<NettingEngineInstance> _nettingEngineInstances;

        public NettingEngineInstanceManager(string[] instances, ILog log)
        {
            _instances = instances;
            _log = log.CreateComponentScope(nameof(NettingEngineInstanceManager));
        }

        public IReadOnlyList<NettingEngineInstance> Instances
        {
            get
            {
                if (_nettingEngineInstances != null)
                    return _nettingEngineInstances;

                lock (_sync)
                {
                    if (_nettingEngineInstances == null)
                    {
                        Initialize();
                    }
                }
                return _nettingEngineInstances;
            }
        }

        public NettingEngineInstance this[int index]
            => Instances[index];

        private void Initialize()
        {
            var instances = new List<NettingEngineInstance>();

            for (var i = 0; i < _instances.Length; i++)
            {
                try
                {
                    var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(_instances[i])
                        //.WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper())
                        .WithoutRetries()
                        .WithoutCaching()
                        .Create();

                    var instance = new NettingEngineInstance(i)
                    {
                        AccountSettings = generator.Generate<IAccountSettingsApi>(),
                        Balances = generator.Generate<IBalancesApi>(),
                        DailyInventory = generator.Generate<IDailyInventoryApi>(),
                        InstrumentInventory = generator.Generate<IInstrumentInventoryApi>(),
                        Instruments = generator.Generate<IInstrumentsApi>(),
                        InstrumentSettings = generator.Generate<IInstrumentSettingsApi>(),
                        Inventory = generator.Generate<IInventoryApi>(),
                        OrderBooks = generator.Generate<IOrderBooksApi>(),
                        Settings = generator.Generate<ISettingsApi>(),
                        Trades = generator.Generate<ITradesApi>(),
                        AssetHedgeSettings = generator.Generate<IAssetHedgeSettingsApi>(),
                        Exchanges = generator.Generate<IExchangesApi>(),
                        ServiceInfo = generator.Generate<IServiceInfoApi>(),
                        ExternalInstruments = generator.Generate<IExternalInstrumentsApi>(),
                        HedgeLimitOrders = generator.Generate<IHedgeLimitOrdersApi>()
                    };

                    ServiceInfoModel serviceInfo = instance.ServiceInfo.GetAsync()
                        .GetAwaiter()
                        .GetResult();

                    instance.DisplayName = serviceInfo.Name;

                    instances.Add(instance);
                }
                catch (Exception exception)
                {
                    _log.WriteWarning(nameof(Initialize), new { instance = _instances[i] }, "Can not create service instance.",
                        exception);
                }
            }

            _nettingEngineInstances = instances;
        }
    }
}
