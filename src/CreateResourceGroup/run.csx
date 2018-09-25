#r "Newtonsoft.Json"

using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");
    AzureCredentials credentials = SdkContext.AzureCredentialsFactory.FromMSI(new MSILoginInformation(MSIResourceType.AppService), AzureEnvironment.AzureGlobalCloud);
    
    var azure = Azure
            .Configure()
            .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
            .Authenticate(credentials)
            .WithDefaultSubscription();

    string requestBody = new StreamReader(req.Body).ReadToEnd();
    var data = JsonConvert.DeserializeObject<CreateResourceGroupRequest>(requestBody, new JsonSerializerSettings
    {
        DateFormatString = "yyyy-MM-dd",
        Culture = CultureInfo.GetCultureInfo("en-US"),
        NullValueHandling = NullValueHandling.Ignore
    });

    if (string.IsNullOrWhiteSpace(data.Location)) data.Location = "West Europe";
    if (data.ExpirationDate == default(DateTime)) data.ExpirationDate = DateTime.UtcNow;


    await azure.ResourceGroups.Define(data.Name).WithRegion(Region.Create(data.Location)).WithTags(new Dictionary<string, string>()
    {
        { "AutoDelete", "true" },
        { "ExpirationDate", data.ExpirationDate.ToString("yyyy-MM-dd") }
    }).CreateAsync();

    return new OkObjectResult("Success");
}


public class CreateResourceGroupRequest
{
    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime ExpirationDate { get; set; }
}