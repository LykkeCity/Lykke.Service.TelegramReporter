using Autofac;
using Lykke.Service.CrossMarketLiquidity.Client;
using Lykke.Service.CrossMarketLiquidity.Client.AutorestClient.Models;
using Lykke.Service.TelegramReporter.Core.Instances;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Services.Instances
{
    public class CrossMarketLiquidityInstanceManager : Dictionary<string, ICrossMarketLiquidityClient>, IStartable,
        ICrossMarketLiquidityInstanceManager
    {
        private readonly ICrossMarketLiquidityClient[] _clients;

        public CrossMarketLiquidityInstanceManager(params ICrossMarketLiquidityClient[] clients)
        {
            _clients = clients;
        }

        public async void Start()
        {
            var tasks = new List<Task<CMLSettingsDto>>();
            foreach (var client in _clients)
            {
                tasks.Add(client.GetCMLSettingsAsync());
            }

            CMLSettingsDto[] settings = await Task.WhenAll(tasks);
            for (int i = 0; i < _clients.Length; i++)
            {
                AddInstance(settings[i].LykkeAssetPair.Name, _clients[i]);
            }
        }

        private void AddInstance(string key, ICrossMarketLiquidityClient client, int? index = null)
        {
            string indexedKey = $"{key}{index.ToString()}";

            if (ContainsKey(indexedKey))
            {
                index = (index ?? 0) + 1;
                AddInstance(key, client, index);
            }
            else
            {
                Add(indexedKey, client);
            }
        }
    }
}
