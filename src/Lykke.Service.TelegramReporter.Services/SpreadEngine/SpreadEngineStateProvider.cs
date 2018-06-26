using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
using Lykke.Service.SpreadEngine.Client.Extensions;
using Lykke.Service.SpreadEngine.Client.Models;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.SpreadEngine;

namespace Lykke.Service.TelegramReporter.Services.SpreadEngine
{
    public class SpreadEngineStateProvider : ISpreadEngineStateProvider
    {
        private readonly ISpreadEngineInstanceManager _spreadEngineInstanceManager;
        private readonly IAssetsService _assetsService;

        public SpreadEngineStateProvider(ISpreadEngineInstanceManager spreadEngineInstanceManager,
            IAssetsService assetsService)
        {
            _spreadEngineInstanceManager = spreadEngineInstanceManager;
            _assetsService = assetsService;
        }

        public async Task<string> GetStateMessageAsync()
        {
            var tasks = new List<Task<string>>();
            foreach (var instanceId in _spreadEngineInstanceManager.Instances.Select(x => x.Index))
            {
                tasks.Add(GetStateMessageAsync(instanceId));
            }

            await Task.WhenAll(tasks);

            var sb = new StringBuilder();
            foreach (var task in tasks)
            {
                sb.AppendLine(task.Result);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public async Task<string> GetStateMessageAsync(int instanceId)
        {
            var taskTraders = _spreadEngineInstanceManager[instanceId]
                .Traders.GetAsync(Array.Empty<string>());

            var balancesTask = _spreadEngineInstanceManager[instanceId]
                .Balances.GetAsync();

            await Task.WhenAll(taskTraders, balancesTask);

            var traders = taskTraders.Result;
            var balances = balancesTask.Result;

            return GetStateMessage(traders, balances);
        }

        private string GetStateMessage(IReadOnlyList<TraderModel> traders, IReadOnlyList<BalanceModel> balances)
        {
            var state = new StringBuilder();

            state.Append($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n\r\n");

            foreach (var trader in traders)
            {
                state.Append(
                    $"{trader.AssetPairId} inv: <abs inventory>; " +
                    $"({trader.SellVolumeCoefficient:0.000}/{trader.BuyVolumeCoefficient:0.000}); " +
                    $"daily: <daily sell volume>/<daily buy volume>; " +
                    $"PL: <PnL>\r\n"
                );
            }

            state.Append("\r\n");
            state.Append("Balances\r\n");

            foreach (var balance in balances)
            {
                state.Append(
                    $"{balance.AssetId} {balance.Amount:0.000}\r\n"
                );
            }

            return state.ToString();
        }
    }
}
