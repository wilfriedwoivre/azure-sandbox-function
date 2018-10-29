using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;

public static async Task Run(TimerInfo myTimer)
{
    AzureCredentials credentials = SdkContext.AzureCredentialsFactory.FromMSI(new MSILoginInformation(MSIResourceType.AppService), AzureEnvironment.AzureGlobalCloud);

    var azure = Azure
        .Configure()
        .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
        .Authenticate(credentials)
        .WithDefaultSubscription();

    var elligibleResourceGroups = await azure.ResourceGroups.ListByTagAsync("AutoDelete", "true", true);

    foreach (var resourceGroup in elligibleResourceGroups)
    {
        if (resourceGroup.Tags.ContainsKey("ExpirationDate"))
        {
            var expirationDate =  resourceGroup.Tags["ExpirationDate"];

            var date = DateTime.Parse(expirationDate, CultureInfo.GetCultureInfo("en-US")); 

            if (date < DateTime.UtcNow)
            {
                azure.ResourceGroups.DeleteByNameAsync(resourceGroup.Name);
            }
        }
    }
}
