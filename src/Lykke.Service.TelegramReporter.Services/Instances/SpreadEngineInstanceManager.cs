using Autofac;
using Lykke.Service.TelegramReporter.Core.Instances;
using System.Collections.Generic;
using Lykke.Service.SpreadEngine.Client.Api;
using Lykke.Service.SpreadEngine.Client.Models;

namespace Lykke.Service.TelegramReporter.Services.Instances
{
    public class SpreadEngineInstanceManager : ISpreadEngineInstanceManager, IStartable
    {
        private readonly string[] _instances;
        private readonly List<SpreadEngineInstance> _spreadEngineInstances;

        public SpreadEngineInstanceManager(string[] instances)
        {
            _instances = instances;
            _spreadEngineInstances = new List<SpreadEngineInstance>();
        }

        public IReadOnlyList<SpreadEngineInstance> Instances
            => _spreadEngineInstances;

        public SpreadEngineInstance this[int index]
            => _spreadEngineInstances[index];

        public void Start()
        {
            _spreadEngineInstances.Clear();

            for (int i = 0; i < _instances.Length; i++)
            {
                var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(_instances[i]).Create();

                var instance = new SpreadEngineInstance(i)
                {
                    Settings = generator.Generate<ISettingsApi>(),
                    Instruments = generator.Generate<IInstrumentsApi>(),
                    Traders = generator.Generate<ITradersApi>(),
                    OrderBooks = generator.Generate<IOrderBooksApi>(),
                    Balances = generator.Generate<IBalancesApi>(),
                };

                ServiceSettingsModel serviceSettings = instance.Settings.GetServiceSettingsAsync()
                    .GetAwaiter()
                    .GetResult();

                instance.DisplayName = serviceSettings.Name;
                instance.Exchanges = serviceSettings.Exchanges;

                _spreadEngineInstances.Add(instance);
            }
        }
    }
}
