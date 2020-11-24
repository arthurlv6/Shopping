using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;

namespace VideoProcessor
{
    public static class WebBackend
    {
        [FunctionName("ProcessCheckoutStarter")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var cart = JsonConvert.DeserializeObject<CartDto>(requestBody);

            if (cart == null || cart.Name == null || cart.Address == null || cart.Lines == null || cart.Lines.Count==0)
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");

            var orchestrationId = await starter.StartNewAsync("O_ProcessCheckout", cart);
            return starter.CreateCheckStatusResponse(req, orchestrationId);
        }
    }
}
