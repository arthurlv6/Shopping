using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Threading;
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
            string paid = null;
            string shipped = null;
            string inventoryDone = null;
            string approvalResult = "Unknown";
            string token = null;
            try
            {

                /*
                if (!ctx.IsReplaying)
                    log.LogInformation("About to call an purchased activity");

                paid = await ctx.CallActivityAsync<string>("A_Payment", cart);

                if (!ctx.IsReplaying)
                    log.LogInformation("About to call shipping");

                shipped = await ctx.CallActivityAsync<string>("A_Shipping", cart);

                if (!ctx.IsReplaying)
                    log.LogInformation("About to call inventory");

                inventoryDone = await ctx.CallActivityAsync<string>("A_Inventory", cart);
                */
                
                

                await ctx.CallActivityAsync("A_SendApprovalRequestEmail", new ApprovalInfo()
                {
                    OrchestrationId = ctx.InstanceId,
                    Message = "activate your account."
                });
                
                using (var cts = new CancellationTokenSource())
                {
                    var timeoutAt = ctx.CurrentUtcDateTime.AddMinutes(5);
                    var timeoutTask = ctx.CreateTimer(timeoutAt, cts.Token);
                    var approvalTask = ctx.WaitForExternalEvent<string>("ApprovalResult");

                    var winner = await Task.WhenAny(approvalTask, timeoutTask);
                    if (winner == approvalTask)
                    {
                        approvalResult = approvalTask.Result;
                        cts.Cancel(); // we should cancel the timeout task
                    }
                    else
                    {
                        approvalResult = "Timed Out";
                    }
                }

                if (approvalResult == "Approved")
                {
                    token = await ctx.CallActivityAsync<string>("A_GetGigyaToken", cart.Name);//Name is the email.
                }
                else
                {
                    await ctx.CallActivityAsync("A_Reject", "rejected");
                }
            }
            catch (Exception e)
            {
                if (!ctx.IsReplaying)
                    log.LogError($"Caught an error from an activity: {e.Message}");

                await
                    ctx.CallActivityAsync<string>("A_Cleanup", 
                        new[] { paid??"", shipped??"", inventoryDone??"" });

                return new
                {
                    Error = "Failed to process the purchase",
                    Message = e.Message,
                    Paid = paid??"",
                    Shipped = shipped??"",
                    InventoryDone = inventoryDone??""
                };
            }

            return new CheckoutReturnDto
            {
                Paid = paid,
                Shipped = shipped,
                InventoryDone = inventoryDone,
                Token = token
            };
        }
    }
}
