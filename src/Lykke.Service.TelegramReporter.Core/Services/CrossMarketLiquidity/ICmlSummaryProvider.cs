using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity
{
    public interface ICmlSummaryProvider
    {
        Task<string> GetSummaryMessageAsync();

        Task<string> GetSummaryMessageAsync(string instanceId);
    }
}
