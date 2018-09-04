using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NettingEngine.Client.RabbitMq;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineAuditPublisher : INettingEngineAuditPublisher
    {
        private readonly INettingEngineAuditProvider _nettingEngineAuditProvider;
        private readonly ITelegramSender _telegramSender;
        private readonly IChatPublisherSettingsRepository _repo;
        private readonly ILog _log;

        public NettingEngineAuditPublisher(ITelegramSender telegramSender,
            INettingEngineAuditProvider nettingEngineAuditProvider,
            IChatPublisherSettingsRepository repo, ILogFactory logFactory)
        {
            _nettingEngineAuditProvider = nettingEngineAuditProvider;
            _telegramSender = telegramSender;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = logFactory.CreateLog(this);
        }

        public async Task Publish(AuditMessage auditMessage)
        {
            try
            {
                var nePublisherSettings = await _repo.GetNeChatPublisherSettings();

                foreach (var settings in nePublisherSettings)
                {
                    await _telegramSender.SendTextMessageAsync(settings.ChatId,
                        await _nettingEngineAuditProvider.GetAuditMessageAsync(auditMessage));
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(NettingEnginePublisher), nameof(Publish), "", ex);
            }
        }
    }
}
