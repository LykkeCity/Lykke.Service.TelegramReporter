using System;
using System.Collections.Generic;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.NettingEngine.Client.Api;
using Lykke.Service.NettingEngine.Client.Models.Settings;
using Lykke.Service.TelegramReporter.Core.Instances;

namespace Lykke.Service.TelegramReporter.Services.Instances
{
    [UsedImplicitly]
    public class NettingEngineInstanceManager : INettingEngineInstanceManager, IStartable
    {
        private readonly string[] _instances;
        private readonly ILog _log;

        private readonly List<NettingEngineInstance> _spreadEngineInstances = new List<NettingEngineInstance>();

        public NettingEngineInstanceManager(string[] instances, ILog log)
        {
            _instances = instances;
            _log = log.CreateComponentScope(nameof(NettingEngineInstanceManager));
        }

        public IReadOnlyList<NettingEngineInstance> Instances
            => _spreadEngineInstances;

        public NettingEngineInstance this[int index]
            => _spreadEngineInstances[index];

        public void Start()
        {
            _spreadEngineInstances.Clear();

            for (int i = 0; i < _instances.Length; i++)
            {
                try
                {
                    var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(_instances[i])
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
                        Trades = generator.Generate<ITradesApi>()
                    };

                    ServiceSettingsModel serviceSettings = instance.Settings.GetAsync()
                        .GetAwaiter()
                        .GetResult();

                    instance.DisplayName = serviceSettings.Name;
                    instance.Exchanges = serviceSettings.Exchanges;

                    _spreadEngineInstances.Add(instance);
                }
                catch (Exception exception)
                {
                    _log.WriteWarning(nameof(Start), new { instance = _instances[i] }, "Can not create service instance.",
                        exception);
                }
            }
        }
    }
}
