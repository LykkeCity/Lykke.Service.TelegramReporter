using System.Timers;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Services
{
    public abstract class ChatPublisher : IStartable, IStopable
    {
        protected readonly ITelegramSender TelegramSender;
        public IChatPublisherSettings PublisherSettings { get; private set; }

        protected readonly ILog Log;

        private Timer _timer;

        protected ChatPublisher(ITelegramSender telegramSender,
            IChatPublisherSettings publisherSettings, ILogFactory logFactory)
        {
            TelegramSender = telegramSender;
            PublisherSettings = publisherSettings;
            Log = logFactory.CreateLog(this);
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
