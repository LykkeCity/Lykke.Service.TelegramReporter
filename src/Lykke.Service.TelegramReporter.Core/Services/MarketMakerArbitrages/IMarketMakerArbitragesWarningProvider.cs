using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Services.MarketMakerArbitrages
{
    public interface IMarketMakerArbitragesWarningProvider
    {
        Task<string> GetWarningMessageAsync(IMarketMakersArbitragesWarning mmArbitrageses);
    }
}
