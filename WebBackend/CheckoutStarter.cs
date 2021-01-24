using System.IO;
using System.Net;
using System.Net.Http;
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

        [FunctionName("SubmitApproval")]
        public static async Task<HttpResponseMessage> SubmitVideoApproval(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SubmitApproval/{id}")]
            HttpRequest req,
           [DurableClient] IDurableOrchestrationClient client,
           [Table("Approvals", "Approval", "{id}", Connection = "AzureWebJobsStorage")] Approval approval,
           ILogger log)
        {
            string result = req.Query["result"];

            if (result == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            log.LogInformation($"Sending approval result to {approval.OrchestrationId} of {result}");

            // send the ApprovalResult external event to this orchestration
            await client.RaiseEventAsync(approval.OrchestrationId, "ApprovalResult", result);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
