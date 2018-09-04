using System;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NettingEngine.Client.RabbitMq;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineAuditProvider : INettingEngineAuditProvider
    {
        private readonly ILog _log;

        public NettingEngineAuditProvider(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
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
            state.AppendLine("Netting Engine audit event:\r\n");

            state.AppendLine($"User: {auditMessage.ClientId}");
            state.Append($"{auditMessage.EventType}");
            if (auditMessage.SettingsChangeType != SettingsChangeType.None)
            {
                state.Append($" ({auditMessage.SettingsChangeType})");
            }
            state.AppendLine(":");
            state.AppendLine();
                       
            switch (auditMessage.EventType)
            {
                case AuditEventType.InstrumentStarted:
                case AuditEventType.InstrumentStopped:
                    state.AppendLine($"{GetInstrument(auditMessage)}");
                    break;
                case AuditEventType.InstrumentSettingsChanged:
                    await HandleInstrumentSettingsChanged(state, GetInstrument(auditMessage), auditMessage);
                    break;
                case AuditEventType.HedgeSettingsChanged:
                    await HandleInstrumentSettingsChanged(state, GetAsset(auditMessage), auditMessage);
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
            var assetDisplayId = GetAssetDisplayId(auditMessage);
            if (!string.IsNullOrWhiteSpace(assetDisplayId))
            {
                state.AppendLine($"Asset DisplayId: {assetDisplayId}");
            }

            var assetPairName = GetAssetPairName(auditMessage);
            if (!string.IsNullOrWhiteSpace(assetPairName))
            {
                state.AppendLine($"AssetPair Name: {assetPairName}");
            }

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

        private string GetInstrument(AuditMessage auditMessage)
        {
            var instrument = auditMessage.CurrentValues?.ContainsKey("InstrumentId") ?? false
                ? $"{auditMessage.CurrentValues["InstrumentId"]}"
                : (auditMessage.PreviousValues?.ContainsKey("InstrumentId") ?? false
                    ? $"{auditMessage.PreviousValues["InstrumentId"]}"
                    : "UNKNOWN_INSTRUMENT");

            return instrument;
        }

        private string GetAsset(AuditMessage auditMessage)
        {
            var asset = auditMessage.CurrentValues?.ContainsKey("Asset") ?? false
                ? $"{auditMessage.CurrentValues["Asset"]}"
                : (auditMessage.PreviousValues?.ContainsKey("Asset") ?? false
                    ? $"{auditMessage.PreviousValues["Asset"]}"
                    : "UNKNOWN_ASSET");

            return asset;
        }

        private string GetAssetDisplayId(AuditMessage auditMessage)
        {
            var asset = auditMessage.CurrentValues?.ContainsKey("DisplayId") ?? false
                ? $"{auditMessage.CurrentValues["DisplayId"]}"
                : (auditMessage.PreviousValues?.ContainsKey("DisplayId") ?? false
                    ? $"{auditMessage.PreviousValues["DisplayId"]}"
                    : null);

            return asset;
        }

        private string GetAssetPairName(AuditMessage auditMessage)
        {
            var asset = auditMessage.CurrentValues?.ContainsKey("Name") ?? false
                ? $"{auditMessage.CurrentValues["Name"]}"
                : (auditMessage.PreviousValues?.ContainsKey("Name") ?? false
                    ? $"{auditMessage.PreviousValues["Name"]}"
                    : null);

            return asset;
        }
    }
}
