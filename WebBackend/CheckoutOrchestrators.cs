using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Threading.Tasks;

namespace WebBackend
{
    public static class CheckoutOrchestrators
    {
        [FunctionName("O_ProcessCheckout")]
        public static async Task<object> ProcessVideo(
            [OrchestrationTrigger] IDurableOrchestrationContext ctx,
            ILogger log)
        {
            var cart = ctx.GetInput<CartDto>();

            if (!ctx.IsReplaying)
                log.LogInformation("About to call transcode video activity");

            string paid = null;
            string shipped = null;
            string inventoryDone = null;

            try
            {
                paid = await
                    ctx.CallActivityAsync<string>("A_Payment", cart);

                if (!ctx.IsReplaying)
                    log.LogInformation("About to call shipping");

                shipped = await
                    ctx.CallActivityAsync<string>("A_Shipping", cart);

                if (!ctx.IsReplaying)
                    log.LogInformation("About to call inventory");

                inventoryDone = await
                    ctx.CallActivityAsync<string>("A_Inventory", cart);
            }
            catch (Exception e)
            {
                if (!ctx.IsReplaying)
                    log.LogInformation($"Caught an error from an activity: {e.Message}");

                await
                    ctx.CallActivityAsync<string>("A_Cleanup", 
                        new[] { paid, shipped, inventoryDone });

                return new
                {
                    Error = "Failed to process uploaded video",
                    Message = e.Message
                };
            }

            return new CheckoutReturnDto
            {
                Paid = paid,
                Shipped = shipped,
                InventoryDone = inventoryDone
            };
        }
    }
}
