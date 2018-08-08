using System.Threading.Tasks;
using Lykke.Service.NettingEngine.Client.RabbitMq;

namespace Lykke.Service.TelegramReporter.Core.Services.NettingEngine
{
    public interface INettingEngineAuditPublisher
    {
        Task Publish(AuditMessage auditMessage);
    }
}
