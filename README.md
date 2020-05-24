# Azure Sandbox

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fwilfriedwoivre%2Fazure-sandbox-function%2Fmaster%2Fazuredeploy.json)

## Application

* Azure Function
    * Create Resource Group : HTTP Trigger
    * Remove Resource Group : Timer Trigger
    * Managed Service Identity

You must assign Managed Service Identity to your target subscription with role **Contributor** or custom role Read, Write, Delete on ResourceGroup

## Create Resource Group

POST METHOD
```json
{ 
     name: "nameValue", 
     expirationDate: "2018-09-25",
     location: "West Europe" 
}
```

Optional values : 
* expirationDate : set current date
* location : West Europe

## Remove Resource Group

TimerTrigger with schedule : __0 0 4 * * *__
