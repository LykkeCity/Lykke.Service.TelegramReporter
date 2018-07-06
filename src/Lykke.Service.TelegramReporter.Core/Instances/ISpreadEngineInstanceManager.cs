using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Core.Instances
{
    public interface ISpreadEngineInstanceManager
    {
        IReadOnlyList<SpreadEngineInstance> Instances { get; }

        SpreadEngineInstance this[int index] { get; }
    }
}
