using System;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.MarketMakerWalletsRebalancer.Contract;
using Lykke.Service.TelegramReporter.Core.Services.WalletsRebalancer;

namespace Lykke.Service.TelegramReporter.Services.WalletsRebalancer
{
    public class WalletsRebalancerProvider : IWalletsRebalancerProvider
    {
        private readonly ILog _log;

        public WalletsRebalancerProvider(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }

        public async Task<string> GetMessageAsync(RebalanceOperation message)
        {
            var messageText = await GetMessage(message);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(messageText);
        }

        private async Task<string> GetMessage(RebalanceOperation message)
        {
            var state = new StringBuilder();

            state.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n");
            state.AppendLine("Wallets Rebalancer event:\r\n");

            

            return state.ToString();
        }
    }
}
