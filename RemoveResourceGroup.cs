using System;
using System.Globalization;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace azure_sandbox_function
{
    public static class RemoveResourceGroup
    {
        [FunctionName("RemoveResourceGroup")]
        public static async Task Run([TimerTrigger("0 0 4 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ArmClient client = new ArmClient(new DefaultAzureCredential());
            SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();
            ResourceGroupCollection resourceGroups = subscription.GetResourceGroups();

            await foreach(var resourceGroup in resourceGroups.GetAllAsync("$filter=AutoDelete eq 'true'")) {
                string expirationDate;
                if (resourceGroup.Data.Tags.TryGetValue("ExpirationDate", out expirationDate)) {
                    var date = DateTime.Parse(expirationDate, CultureInfo.GetCultureInfo("en-US")); 
                    if (date < DateTime.UtcNow) {
                        await resourceGroup.DeleteAsync(Azure.WaitUntil.Started);
                    }
                }
            }
            
            
        }

    }
}
