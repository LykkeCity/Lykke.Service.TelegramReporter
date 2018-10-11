using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Services.LquidityEngineTrades
{
    public class LiquidityEngineTradesPublisher : ChatPublisher
    {
        public LiquidityEngineTradesPublisher(ITelegramSender telegramSender, IChatPublisherSettings publisherSettings, ILogFactory logFactory) 
            : base(telegramSender, publisherSettings, logFactory)
        {
        }

        public override async void Publish()
        {
            await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId, "Hello world");
        }
    }
}
