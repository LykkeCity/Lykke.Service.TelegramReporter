using System.Timers;
using Autofac;
using Common;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Settings;

namespace Lykke.Service.TelegramReporter.Services
{
    public abstract class ChatPublisher : IStartable, IStopable
    {
        protected readonly ITelegramSender TelegramSender;
        protected readonly PublisherSettings PublisherSettings;

        private Timer _timer;

        protected ChatPublisher(ITelegramSender telegramSender,
            PublisherSettings publisherSettings)
        {
            TelegramSender = telegramSender;
            PublisherSettings = publisherSettings;
        }

        public void Start()
        {
            if (PublisherSettings == null || PublisherSettings.ChatId == 0)
            {
                return;
            }

            _timer = new Timer(PublisherSettings.TimeSpan.TotalMilliseconds);
            _timer.Elapsed += (sender, e) => Publish();
            _timer.Start();
        }

        public abstract void Publish();

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
