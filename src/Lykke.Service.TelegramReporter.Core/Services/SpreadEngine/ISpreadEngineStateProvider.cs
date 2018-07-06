using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.SpreadEngine
{
    public interface ISpreadEngineStateProvider
    {
        Task<string> GetStateMessageAsync();
        Task<string> GetStateMessageAsync(int instanceId);
    }
}
