using Autofac;
using Common;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Core.Settings;
using System.Timers;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlPublisher : IStartable, IStopable
    {
        private readonly ITelegramSender _telegramSender;
        private readonly ICmlSummaryProvider _cmlSummaryProvider;
        private readonly ICmlStateProvider _cmlStateProvider;
        private readonly PublisherSettings _publisherSettings;

        private Timer _timer;

        public CmlPublisher(ITelegramSender telegramSender,
            ICmlSummaryProvider cmlSummaryProvider,
            ICmlStateProvider cmlStateProvider,
            PublisherSettings publisherSettings)
        {
            _telegramSender = telegramSender;
            _cmlSummaryProvider = cmlSummaryProvider;
            _cmlStateProvider = cmlStateProvider;
            _publisherSettings = publisherSettings;
        }

        public void Start()
        {
            _timer = new Timer(_publisherSettings.TimeSpan.TotalMilliseconds);
            _timer.Elapsed += (sender, e) => Publish();
            _timer.Start();
        }

        public async void Publish()
        {
            await _telegramSender.SendTextMessageAsync(_publisherSettings.ChatId,
                await _cmlSummaryProvider.GetSummaryMessageAsync());

            await _telegramSender.SendTextMessageAsync(_publisherSettings.ChatId,
                await _cmlStateProvider.GetStateMessageAsync());
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Stop()
        {
            _timer?.Stop();
        }
    }
}
