using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Core
{
    public abstract class ChatPublisher : IStartable, IStopable
    {
        protected readonly ITelegramSender TelegramSender;
        public IChatPublisherSettings PublisherSettings { get; private set; }

        protected readonly ILog Log;

        private readonly TimerTrigger _timer;

        protected ChatPublisher(ITelegramSender telegramSender,
            IChatPublisherSettings publisherSettings, ILogFactory logFactory)
        {
            TelegramSender = telegramSender;
            PublisherSettings = publisherSettings;
            Log = logFactory.CreateLog(this);
            _timer = new TimerTrigger(this.GetType().FullName, PublisherSettings.TimeSpan, logFactory, TimeHendler);
        }

        private async Task TimeHendler(ITimerTrigger timer, TimerTriggeredHandlerArgs args, CancellationToken cancellationtoken)
        {
            await Publish();
        }

        public void Start()
        {
            if (PublisherSettings == null || PublisherSettings.ChatId == 0)
            {
                return;
            }

            _timer.Start();

            Log.Info($"Start work ChatPublisher: {this.GetType().FullName} with timeout: {PublisherSettings?.TimeSpan}");
        }

        public abstract Task Publish();

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Stop()
        {
            Log.Info($"Stop work ChatPublisher: {this.GetType().FullName} with timeout: {PublisherSettings?.TimeSpan}");
            _timer?.Stop();
        }
    }
}
