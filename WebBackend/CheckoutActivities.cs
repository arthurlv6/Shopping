using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Shared;
using System;
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
    }
}
