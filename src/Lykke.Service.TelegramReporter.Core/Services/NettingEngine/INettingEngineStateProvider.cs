using System.Threading.Tasks;
using Lykke.Service.MarketMakerReports.Client.Models.InventorySnapshots;

namespace Lykke.Service.TelegramReporter.Core.Services.NettingEngine
{
    public interface INettingEngineStateProvider
    {
        Task<string> GetStateMessageAsync(InventorySnapshotModel prevSnapshot, InventorySnapshotModel snapshot);
    }
}
