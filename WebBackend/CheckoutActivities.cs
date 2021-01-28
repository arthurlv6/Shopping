using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using Shared;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace WebBackend
{
    public class CheckoutActivities
    {
        private InventoryService _inventoryService;
        public CheckoutActivities(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [FunctionName("A_Payment")]
        public static async Task<string> Payment([ActivityTrigger] CartDto cart,ILogger log)
        {
            log.LogInformation($"payment processed cart {cart.Name}");
            // simulate doing the activity
            await Task.Delay(1000);
            return $"payemnt Approved";
        }

        [FunctionName("A_Shipping")]
        public async Task<string> Shipping([ActivityTrigger] CartDto cart,ILogger log)
        {
            log.LogInformation($"shipping processed cart {cart.Name}");

            if (cart.Name.Contains("error"))
            {
                throw new InvalidOperationException("Failed");
            }
            // simulate doing the activity
            await Task.Delay(1000);

            return "Shipping approved";
        }

        [FunctionName("A_Inventory")]
        public async Task<string> Inventory([ActivityTrigger] CartDto cart,ILogger log)
        {
            await _inventoryService.PostAsync(cart);
            log.LogInformation($"inventory processed cart");
            return "inventory approved";
        }

        [FunctionName("A_Cleanup")]
        public async Task<string> Cleanup([ActivityTrigger] string[] toCleanUp,ILogger log)
        {
            foreach (var file in toCleanUp.Where(f => f != null))
            {
                log.LogInformation($"Deleting {file}");
                // simulate doing the activity
                await Task.Delay(1000);
            }
            return "Cleaned up successfully";
        }

        [FunctionName("A_SendApprovalRequestEmail")]
        public static void SendApprovalRequestEmail(
            [ActivityTrigger] ApprovalInfo approvalInfo,
            [SendGrid(ApiKey = "SendGridKey")] out SendGridMessage message,
            [Table("Approvals", "AzureWebJobsStorage")] out Approval approval,
            ILogger log)
        {
            var approvalCode = Guid.NewGuid().ToString("N");
            approval = new Approval
            {
                PartitionKey = "Approval",
                RowKey = approvalCode,
                OrchestrationId = approvalInfo.OrchestrationId
            };
            var subject = "An activity is awaiting approval";
            
            var host = Environment.GetEnvironmentVariable("Host");
            var functionAddress = $"{host}/api/SubmitApproval/{approvalCode}";
            var approvedLink = functionAddress + "?result=Approved";
            var rejectedLink = functionAddress + "?result=Rejected";
            var body = $"Please {approvalInfo.Message}<br>"
                               + $"<a href=\"{approvedLink}\">Approve</a><br>"
                               + $"<a href=\"{rejectedLink}\">Reject</a>";

            message = new SendGridMessage();
            message.AddTo(Environment.GetEnvironmentVariable("ApproverEmail"));
            message.AddContent("text/html", body);
            message.SetFrom(new EmailAddress(Environment.GetEnvironmentVariable("SenderEmail")));
            message.SetSubject(subject);
        }
        [FunctionName("A_Approve")]
        public static async Task Approve(
            [ActivityTrigger] string input,
            ILogger log)
        {
            log.LogInformation($"Publishing {input}");
            await Task.Delay(1000);
        }

        [FunctionName("A_Reject")]
        public static async Task Reject(
            [ActivityTrigger] string input,
            ILogger log)
        {
            log.LogInformation($"Rejecting {input}");
            await Task.Delay(1000);
        }
    }
}
