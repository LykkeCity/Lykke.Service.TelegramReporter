using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.MarketMakerReports.Client.Models.InventorySnapshots;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Services.NettingEngine;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.TelegramReporter.Controllers
{
    public class NettingEngineTestController : Controller
    {
        private IChatPublisherStateService _chatPublisherStateService;

        public NettingEngineTestController(IChatPublisherStateService chatPublisherStateService)
        {
            _chatPublisherStateService = chatPublisherStateService;
        }

        [HttpPost("nepushmessage/{id}")]
        public IActionResult NePushMessage([FromRoute]string id)
        {
            var idPublisher = long.Parse(id);
            var publisher = _chatPublisherStateService.NePublishers[idPublisher] as NettingEnginePublisher;
            publisher.Publish();

            return Ok();
        }

        [HttpGet("negetprevshapshot/{id}")]
        public IActionResult GetPrevSnapshot([FromRoute]string id)
        {
            var idPublisher = long.Parse(id);
            var publisher = _chatPublisherStateService.NePublishers[idPublisher] as NettingEnginePublisher;
            return Ok(publisher.PrevSnapshot);
        }

        [HttpGet("negetcurrentshapshot/{id}")]
        public async Task<IActionResult> GetCurrentSnapshot([FromRoute]string id)
        {
            var idPublisher = long.Parse(id);
            var publisher = _chatPublisherStateService.NePublishers[idPublisher] as NettingEnginePublisher;
            var snapshot = await publisher.GetCurrentInventorySnapshot();
            return Ok(snapshot);
        }

        [HttpGet("negetpublishelist")]
        public IActionResult GetPublisheList()
        {
            var list = _chatPublisherStateService.NePublishers.Where(e => e.Value is NettingEnginePublisher).Select(e => e.Key).ToArray();

            return Ok(list);
        }
    }
}
