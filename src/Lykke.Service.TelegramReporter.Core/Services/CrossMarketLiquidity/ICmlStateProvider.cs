using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity
{
    public interface ICmlStateProvider
    {
        Task<string> GetStateMessageAsync();
        Task<string> GetStateMessageAsync(string instanceId);
    }
}
