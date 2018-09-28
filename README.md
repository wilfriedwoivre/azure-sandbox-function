# Azure Sandbox

[![Deploy to Azure](https://azuredeploy.net/deploybutton.svg)](https://azuredeploy.net/?repository=https://github.com/wilfriedwoivre/azure-sandbox-function/tree/master)

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
