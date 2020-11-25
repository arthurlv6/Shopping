using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inventory
{
    public class FunAPI
    {
        private readonly StockService _service;
        public FunAPI(StockService service)
        {
            _service = service;
        }
        [FunctionName("InventoryPost")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var result = _service.NewOrder(req);
            if (result.IsSuccess)
                return new OkObjectResult("done");
            return new BadRequestResult();
        }
    }
}
