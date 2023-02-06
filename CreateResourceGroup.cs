using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.Core;
using System.Collections.Generic;
using Azure;

namespace azure_sandbox_function
{
    public static class CreateResourceGroup
    {
        [FunctionName("CreateResourceGroup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var data = JsonConvert.DeserializeObject<CreateResourceGroupRequest>(requestBody, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                Culture = CultureInfo.GetCultureInfo("en-US"),
                NullValueHandling = NullValueHandling.Ignore
            });

            if (data.ExpirationDate == default(DateTime)) data.ExpirationDate = DateTime.UtcNow;

            ArmClient client = new ArmClient(new DefaultAzureCredential());
            SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();
            ResourceGroupCollection resourceGroups = subscription.GetResourceGroups();

            AzureLocation location = AzureLocation.WestEurope;
            string resourceGroupName = data.Name;

            ResourceGroupData resourceGroupData = new ResourceGroupData(location);
            ArmOperation<ResourceGroupResource> resourceGroupOperation = await resourceGroups.CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, resourceGroupData);
            ResourceGroupResource resourceGroup = resourceGroupOperation.Value;

            var tags = new Dictionary<string, string>()
            {
                { "AutoDelete", "true" },
                { "ExpirationDate", data.ExpirationDate.ToString("yyyy-MM-dd") }
            };
            await resourceGroup.SetTagsAsync(tags);

            return new OkObjectResult("Success");
        }
    }

    public class CreateResourceGroupRequest
    {
        public string Name { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
