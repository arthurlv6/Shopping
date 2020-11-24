using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using System.Linq;

namespace Inventory
{
    public class FunAPI
    {
        private readonly StockRepo repo;
        public FunAPI(StockRepo _repo)
        {
            repo = _repo;
        }
        [FunctionName("InventoryPost")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<CartDto>(requestBody);
                var theFirstLine = data.Lines.FirstOrDefault();
                StockDto stockModel = new StockDto() { Name = theFirstLine.Name, Quantity = theFirstLine.Quantity };
                await repo.AddAsync(stockModel);
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                log.LogCritical(ex.Message);
                return new BadRequestResult();
            }
            return new OkObjectResult("done");
        }
    }
}
