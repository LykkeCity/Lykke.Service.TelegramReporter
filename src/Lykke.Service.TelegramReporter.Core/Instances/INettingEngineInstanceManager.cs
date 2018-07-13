using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Core.Instances
{
    public interface INettingEngineInstanceManager
    {
        IReadOnlyList<NettingEngineInstance> Instances { get; }

        NettingEngineInstance this[int index] { get; }
    }
}
