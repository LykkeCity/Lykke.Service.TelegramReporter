using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.NettingEngine
{
    public interface INettingEngineStateProvider
    {
        Task<string> GetStateMessageAsync();
        Task<string> GetStateMessageAsync(int instanceId);
    }
}
