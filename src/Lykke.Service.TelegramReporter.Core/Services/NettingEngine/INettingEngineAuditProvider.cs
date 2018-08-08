using System.Threading.Tasks;
using Lykke.Service.NettingEngine.Client.RabbitMq;

namespace Lykke.Service.TelegramReporter.Core.Services.NettingEngine
{
    public interface INettingEngineAuditProvider
    {
        Task<string> GetAuditMessageAsync(AuditMessage auditMessage);
    }
}
