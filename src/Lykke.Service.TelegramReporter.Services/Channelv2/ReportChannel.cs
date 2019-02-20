using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;

namespace Lykke.Service.TelegramReporter.Services.Channelv2
{
    public abstract class ReportChannel : IReportChannel
    {
        private readonly ITelegramSender _telegramSender;
        private readonly ILogFactory _logFactory;
        private TimerTrigger _timer;

        protected ReportChannel(IReportChannel channel, ITelegramSender telegramSender, ILogFactory logFactory)
        {
            _telegramSender = telegramSender;
            _logFactory = logFactory;
            ChannelId = channel.ChannelId;
            Type = channel.Type;
            ChatId = channel.ChatId;
            Interval = channel.Interval;
            Metainfo = channel.Metainfo;

            Log = _logFactory.CreateLog(this);
        }

        protected ILog Log { get; }

        public string ChannelId { get; }
        public string Type { get; }
        public long ChatId { get; }
        public TimeSpan Interval { get; }
        public string Metainfo { get; }

        public void Start()
        {
            _timer = new TimerTrigger(GetType().Name, Interval, _logFactory, Handler);
            _timer.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        private async Task Handler(ITimerTrigger timer, TimerTriggeredHandlerArgs args, CancellationToken cancellationtoken)
        {
            try
            {
                await DoTimer();
            }
            catch (Exception ex)
            {
                Log.Error(ex, context: $"Metainfo: {Metainfo}");
            }
            
        }

        protected abstract Task DoTimer();

        protected Task SendMessage(string message)
        {
            return _telegramSender.SendTextMessageAsync(ChatId, message);
        }
    }
}
