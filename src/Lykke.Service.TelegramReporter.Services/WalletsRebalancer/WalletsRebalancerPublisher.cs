using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.MarketMakerWalletsRebalancer.Contract;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.WalletsRebalancer;

namespace Lykke.Service.TelegramReporter.Services.WalletsRebalancer
{
    public class WalletsRebalancerPublisher : IWalletsRebalancerPublisher
    {
        private readonly IWalletsRebalancerProvider _walletsRebalancerProvider;
        private readonly ITelegramSender _telegramSender;
        private readonly IChatPublisherSettingsRepository _repo;
        private readonly ILog _log;

        public WalletsRebalancerPublisher(ITelegramSender telegramSender,
            IWalletsRebalancerProvider walletsRebalancerProvider,
            IChatPublisherSettingsRepository repo, ILogFactory logFactory)
        {
            _walletsRebalancerProvider = walletsRebalancerProvider;
            _telegramSender = telegramSender;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = logFactory.CreateLog(this);
        }

        public async Task Publish(RebalanceOperation message)
        {
            _log.Info("Wallets Rebalancer Publisher - publishing a message...", message);

            try
            {
                var publisherSettings = await _repo.GetWalletsRebalancerChatPublisherSettings();

                foreach (var settings in publisherSettings)
                {
                    await _telegramSender.SendTextMessageAsync(settings.ChatId,
                        await _walletsRebalancerProvider.GetMessageAsync(message));
                }

                _log.Info("Wallets Rebalancer Publisher - successfully published a message.", message);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsRebalancerPublisher), nameof(Publish), "", ex);
            }
        }
    }
}
