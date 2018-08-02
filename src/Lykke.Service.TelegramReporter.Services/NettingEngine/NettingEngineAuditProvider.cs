using System;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.NettingEngine.Client.RabbitMq;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineAuditProvider : INettingEngineAuditProvider
    {
        private readonly ILog _log;

        public NettingEngineAuditProvider(ILog log)
        {
            _log = log;
        }

        public async Task<string> GetAuditMessageAsync(AuditMessage auditMessage)
        {
            var message = await GetAuditMessage(auditMessage);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(message);
        }

        private async Task<string> GetAuditMessage(AuditMessage auditMessage)
        {
            var state = new StringBuilder();

            state.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n");
            state.AppendLine($"NettingsEngine audit event:\r\n");

            state.AppendLine($"User: {auditMessage.ClientId}");
            state.Append($"{auditMessage.EventType}");
            if (auditMessage.SettingsChangeType != SettingsChangeType.None)
            {
                state.Append($" ({auditMessage.SettingsChangeType})");
            }
            state.AppendLine(":");
            state.AppendLine();

            var instrument = auditMessage.CurrentValues.ContainsKey("InstrumentId")
                ? $"{auditMessage.CurrentValues["InstrumentId"]}"
                : "UNKNOWN_INSTRUMENT";

            var asset = auditMessage.CurrentValues.ContainsKey("Asset")
                ? $"{auditMessage.CurrentValues["Asset"]}"
                : "UNKNOWN_ASSET";

            switch (auditMessage.EventType)
            {
                case AuditEventType.InstrumentStarted:
                case AuditEventType.InstrumentStopped:
                    state.AppendLine($"{instrument}");
                    break;
                case AuditEventType.InstrumentSettingsChanged:
                    await HandleInstrumentSettingsChanged(state, instrument, auditMessage);
                    break;
                case AuditEventType.HedgeSettingsChanged:
                    await HandleInstrumentSettingsChanged(state, asset, auditMessage);
                    break;
                default:
                    await _log.WriteWarningAsync(nameof(NettingEngineAuditProvider), nameof(GetAuditMessage), $"auditMessage: {auditMessage.ToJson()}",
                        "Unrecognized AuditEventType in audit message.");
                    break;
            }

            return state.ToString();
        }

        private async Task HandleInstrumentSettingsChanged(StringBuilder state, string name, AuditMessage auditMessage)
        {
            if (auditMessage.SettingsChangeType == SettingsChangeType.Modified)
            {
                foreach (var key in auditMessage.PreviousValues.Keys)
                {
                    if (auditMessage.CurrentValues[key] != auditMessage.PreviousValues[key])
                    {
                        state.AppendLine(
                            $"{name}-{key} {auditMessage.CurrentValues[key]} ({auditMessage.PreviousValues[key]})");
                    }
                }
            }
            else if (auditMessage.SettingsChangeType == SettingsChangeType.Created ||
                     auditMessage.SettingsChangeType == SettingsChangeType.Deleted)
            {
                state.AppendLine($"{name}");
            }
            else
            {
                await _log.WriteWarningAsync(nameof(NettingEngineAuditProvider), nameof(GetAuditMessage),
                    $"auditMessage: {auditMessage.ToJson()}",
                    "Unrecognized SettingsChangeType in audit message.");
            }
        }
    }
}
