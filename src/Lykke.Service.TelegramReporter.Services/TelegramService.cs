using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Settings;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Service.TelegramReporter.Services
{
    public class TelegramService : IStartable, IStopable
    {
        private const string cmlSummaryCommand = "/cmlsummary";

        private TelegramSettings _settings;
        private TelegramBotClient _client;
        private ICMLSummaryProvider _cmlSummaryProvider;
        private ILog _log;

        public TelegramService(TelegramSettings settings, ICMLSummaryProvider cmlSummaryProvider, ILog log)
        {
            _settings = settings;
            _cmlSummaryProvider = cmlSummaryProvider;
            _log = log;
        }

        public void Start()
        {
            var proxy = new HttpToSocks5Proxy(_settings.ProxySettings.Host, _settings.ProxySettings.Port,
                _settings.ProxySettings.Username, _settings.ProxySettings.Password);
            _client = new TelegramBotClient(_settings.TelegramToken, proxy);
            _client.SetWebhookAsync("").GetAwaiter().GetResult();            
            _client.OnMessage += ClientOnOnMessage;
            _client.OnMessageEdited += ClientOnOnMessage;

            _client.StartReceiving(new[] { UpdateType.Message });
        }

        private async void ClientOnOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                string command = messageEventArgs.Message.Text.ToLower().Trim().Split(' ').FirstOrDefault();
                switch (command)
                {
                    case cmlSummaryCommand:
                        await SendMessageAsync(await _cmlSummaryProvider.GetSummaryMessageAsync(),
                            messageEventArgs.Message);
                        break;
                    default:
                        await SendMessageAsync("Unknown command", messageEventArgs.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(TelegramService), nameof(ClientOnOnMessage),
                    messageEventArgs.ToJson(), ex);
            }
        }

        private async Task SendMessageAsync(string text, Message message)
        {
            _client.SendTextMessageAsync(message.Chat.Id, text,
                parseMode: ParseMode.Html,
                replyToMessageId: message.MessageId).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            _client.StopReceiving();
        }
    }
}
