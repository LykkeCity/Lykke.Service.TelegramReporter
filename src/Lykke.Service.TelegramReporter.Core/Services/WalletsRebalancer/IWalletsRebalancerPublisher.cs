using System.Threading.Tasks;
using Lykke.Service.MarketMakerWalletsRebalancer.Contract;

namespace Lykke.Service.TelegramReporter.Core.Services.WalletsRebalancer
{
    public interface IWalletsRebalancerPublisher
    {
        Task Publish(RebalanceOperation message);
    }
}
