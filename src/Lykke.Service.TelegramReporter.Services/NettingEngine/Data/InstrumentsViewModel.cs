using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class InstrumentsViewModel : InstanceViewModel
    {
        public InstrumentsViewModel()
        {
        }

        public InstrumentsViewModel(int serviceInstanceIndex)
            : base(serviceInstanceIndex)
        {
        }

        public InstrumentSettingsViewModel DefaultSettings { get; set; }

        public IReadOnlyList<AssetViewModel> Assets { get; set; }

        public IReadOnlyList<AssetPairViewModel> AssetPairs { get; set; }

        public IReadOnlyList<ItemViewModel> Exchanges { get; set; }

        public IReadOnlyList<AssetInventoryModel> AssetInventory { get; set; }
    }
}
