using Autofac;
using Common;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Settings;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Service.TelegramReporter.Services
{
    public class TelegramService : IStartable, IStopable, ITelegramSender
    {    
        private readonly TelegramSettings _settings;
        private readonly IEnumerable<ITelegramSubscriber> _subscribers;
        private readonly ILog _log;
        private TelegramBotClient _client;

        public TelegramService(TelegramSettings settings, 
                                IEnumerable<ITelegramSubscriber> subscribers,
                                ILog log)
        {
            _settings = settings;
            _subscribers = subscribers;
            _log = log;
        }

        public void Start()
        {
            if (_settings.ProxySettings == null || string.IsNullOrEmpty(_settings.ProxySettings.Host))
            {
                _client = new TelegramBotClient(_settings.TelegramToken);
            }
            else
            {
                var proxy = new HttpToSocks5Proxy(_settings.ProxySettings.Host, _settings.ProxySettings.Port,
                    _settings.ProxySettings.Username, _settings.ProxySettings.Password);
                _client = new TelegramBotClient(_settings.TelegramToken, proxy);
            }

            _client.SetWebhookAsync("").GetAwaiter().GetResult();
            _client.OnMessage += ClientOnOnMessage;
            _client.OnMessageEdited += ClientOnOnMessage;
            _client.OnCallbackQuery += ClientOnOnCallbackQuery;
            _client.OnReceiveError += ClientOnOnReceiveError;
            _client.OnReceiveGeneralError += ClientOnOnReceiveGeneralError;

            _client.StartReceiving(Array.Empty<UpdateType>());
        }

        private void ClientOnOnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs receiveGeneralErrorEventArgs)
        {
            _log.WriteErrorAsync(nameof(TelegramService), nameof(ClientOnOnReceiveGeneralError),
                null, receiveGeneralErrorEventArgs.Exception);
        }

        private void ClientOnOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            _log.WriteErrorAsync(nameof(TelegramService), nameof(ClientOnOnReceiveError),
                null, receiveErrorEventArgs.ApiRequestException);
        }

        private async void ClientOnOnCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            try
            {
                var callbackQuery = callbackQueryEventArgs.CallbackQuery;
                await _client.SendChatActionAsync(callbackQuery.Message.Chat.Id, ChatAction.Typing);

                string command = ExtractCommand(callbackQuery.Data);

                var tasks = new List<Task>();
                foreach (ITelegramSubscriber subscriber in _subscribers)
                {
                    if (string.Equals(command, subscriber.Command, StringComparison.OrdinalIgnoreCase))
                    {
                        tasks.Add(subscriber.ProcessCallbackQuery(this, callbackQuery));
                    }
                }

                if (tasks.Any())
                {
                    await Task.WhenAll(tasks);
                }
                else
                {
                    await SendMessageAsync("Unknown command", callbackQuery.Message);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(TelegramService), nameof(ClientOnOnCallbackQuery),
                    callbackQueryEventArgs.ToJson(), ex);
            }
        }

        private async void ClientOnOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                await _client.SendChatActionAsync(messageEventArgs.Message.Chat.Id, ChatAction.Typing);

                string command = ExtractCommand(messageEventArgs.Message.Text);
                var tasks = new List<Task>();
                foreach (ITelegramSubscriber subscriber in _subscribers)
                {
                    if (string.Equals(command, subscriber.Command, StringComparison.OrdinalIgnoreCase))
                    {
                        tasks.Add(subscriber.ProcessMessageAsync(this, messageEventArgs.Message));
                    }
                }

                if (tasks.Any())
                {
                    await Task.WhenAll(tasks);
                }
                else
                {
                    await SendMessageAsync("Unknown command", messageEventArgs.Message);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(TelegramService), nameof(ClientOnOnMessage),
                    messageEventArgs.ToJson(), ex);
            }
        }

        private static readonly Regex _commandRegex = new Regex(@"(?<command>/\w+)", RegexOptions.Multiline);
        private string ExtractCommand(string message)
        {
            Match match = _commandRegex.Match(message);
            if (!match.Success)
                return null;

            Group group = match.Groups["command"];
            if (!group.Success)
                return null;

            return group.Value;
        }

        private Task SendMessageAsync(string text, Message message)
        {
            return SendTextMessageAsync(message.Chat.Id, text,
                replyToMessageId: message.MessageId);
        }

        public Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _client.SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview, 
                disableNotification, replyToMessageId, replyMarkup, cancellationToken);
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
