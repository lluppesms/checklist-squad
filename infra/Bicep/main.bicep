// --------------------------------------------------------------------------------
// Main Bicep file that creates all of the Azure Resources for one environment
// --------------------------------------------------------------------------------
// To deploy this Bicep manually:
//   az login
//   az account set --subscription <subscriptionId>
//   az deployment group create -n "manual-$(Get-Date -Format 'yyyyMMdd-HHmmss')" `
//     --resource-group rg-checklist-dev --template-file 'main.bicep' --parameters appName=xxx environmentCode=dev adminUserId=xxxxxxxx-xxxx-xxxx
// --------------------------------------------------------------------------------
param appName string

@allowed(['dev', 'qa', 'prod'])
@description('The environment code (dev, qa, prod).')
param environmentCode string = 'dev'

@description('The Azure region for resource deployment.')
param location string = resourceGroup().location

@description('The instance number for multiple deployments.')
param instanceNumber string = '1'

@description('Deploy only website infrastructure (skip SQL resources).')
param websiteOnly bool = false

@description('Name of a pre-existing App Service Plan to use instead of creating a new one.')
param servicePlanName string = ''

@description('Resource group of a pre-existing App Service Plan.')
param servicePlanResourceGroupName string = ''

@description('The kind of web app (linux or windows).')
param webAppKind string = 'linux'

@description('The SKU for the App Service Plan when creating a new one.')
@allowed(['F1','B1','B2','S1','S2','S3'])
param webSiteSku string = 'B1'

@description('The name of the SQL database.')
param sqlDatabaseName string = 'CheckListDb'

@description('Name of an existing SQL Server to use instead of creating a new one.')
param existingSqlServerName string = ''

@description('Resource group of an existing SQL Server.')
param existingSqlServerResourceGroupName string = ''

@description('The AAD admin login user ID (email).')
param sqlAdminLoginUserId string = ''

@description('The AAD admin login user SID (object ID).')
param sqlAdminLoginUserSid string = ''

@description('The AAD admin login tenant ID.')
param sqlAdminLoginTenantId string = ''

@description('The Key Vault owner user ID (object ID).')
param adminUserId string = ''

// calculated variables disguised as parameters
param runDateTime string = utcNow()

// --------------------------------------------------------------------------------
var deploymentSuffix = '-${runDateTime}'
var commonTags = {
  LastDeployed: runDateTime
  Application: appName
  Environment: environmentCode
}

// --------------------------------------------------------------------------------
// Resource Names
// --------------------------------------------------------------------------------
module resourceNames 'resourcenames.bicep' = {
  name: 'resourcenames${deploymentSuffix}'
  params: {
    appName: appName
    environmentCode: environmentCode
    instanceNumber: instanceNumber
  }
}

// --------------------------------------------------------------------------------
// Monitoring: Log Analytics + App Insights
// --------------------------------------------------------------------------------
module monitorModule 'modules/monitor/monitor.bicep' = {
  name: 'monitor${deploymentSuffix}'
  params: {
    logAnalyticsWorkspaceName: resourceNames.outputs.logAnalyticsWorkspaceName
    appInsightsName: resourceNames.outputs.webSiteAppInsightsName
    location: location
    commonTags: commonTags
  }
}

// --------------------------------------------------------------------------------
// Database: Azure SQL Server + Database
// --------------------------------------------------------------------------------
module databaseModule 'modules/database/sqlserver.bicep' = if (!websiteOnly) {
  name: 'database${deploymentSuffix}'
  params: {
    sqlServerName: resourceNames.outputs.sqlServerName
    sqlDatabaseName: sqlDatabaseName
    existingSqlServerName: existingSqlServerName
    existingSqlServerResourceGroupName: existingSqlServerResourceGroupName
    location: location
    commonTags: commonTags
    environmentCode: environmentCode
    adAdminLoginUserId: sqlAdminLoginUserId
    adAdminLoginUserSid: sqlAdminLoginUserSid
    adAdminLoginTenantId: sqlAdminLoginTenantId
    workspaceId: monitorModule.outputs.logAnalyticsWorkspaceId
  }
}

// --------------------------------------------------------------------------------
// SignalR Service
// --------------------------------------------------------------------------------
module signalRModule 'modules/signalr/signalr.bicep' = {
  name: 'signalr${deploymentSuffix}'
  params: {
    signalRServiceName: resourceNames.outputs.signalRServiceName
    location: location
    commonTags: commonTags
    environmentCode: environmentCode
  }
}

// --------------------------------------------------------------------------------
// App Service Plan (new or existing)
// --------------------------------------------------------------------------------
module appServicePlanModule 'modules/webapp/websiteserviceplan.bicep' = {
  name: 'appServicePlan${deploymentSuffix}'
  params: {
    location: location
    commonTags: commonTags
    sku: webSiteSku
    appServicePlanName: servicePlanName == '' ? resourceNames.outputs.webSiteAppServicePlanName : servicePlanName
    existingServicePlanName: servicePlanName
    existingServicePlanResourceGroupName: servicePlanResourceGroupName
    webAppKind: webAppKind
  }
}

// --------------------------------------------------------------------------------
// Web App
// --------------------------------------------------------------------------------
var webAppConnectionString = !websiteOnly ? databaseModule!.outputs.connectionString : ''

module webAppModule 'modules/webapp/webapp.bicep' = {
  name: 'webapp${deploymentSuffix}'
  params: {
    webSiteName: resourceNames.outputs.webSiteName
    location: location
    commonTags: commonTags
    environmentCode: environmentCode
    appInsightsConnectionString: monitorModule.outputs.appInsightsConnectionString
    appInsightsInstrumentationKey: monitorModule.outputs.appInsightsInstrumentationKey
    workspaceId: monitorModule.outputs.logAnalyticsWorkspaceId
    appServicePlanName: appServicePlanModule.outputs.name
    appServicePlanResourceGroupName: appServicePlanModule.outputs.resourceGroupName
    webAppKind: webAppKind
    customAppSettings: {
      ASPNETCORE_ENVIRONMENT: environmentCode == 'prod' ? 'Production' : 'Development'
      ConnectionStrings__DefaultConnection: webAppConnectionString
      Azure__SignalR__ConnectionString: signalRModule.outputs.signalRConnectionString
    }
  }
}

// --------------------------------------------------------------------------------
// Security: Key Vault with secrets
// --------------------------------------------------------------------------------
module keyVaultModule 'modules/security/keyvault.bicep' = {
  name: 'keyvault${deploymentSuffix}'
  params: {
    keyVaultName: resourceNames.outputs.keyVaultName
    location: location
    commonTags: commonTags
    keyVaultOwnerUserId: adminUserId
    applicationUserObjectIds: [webAppModule.outputs.principalId]
    workspaceId: monitorModule.outputs.logAnalyticsWorkspaceId
    sqlConnectionString: webAppConnectionString
    signalRConnectionString: signalRModule.outputs.signalRConnectionString
    publicNetworkAccess: 'Enabled'
    allowNetworkAccess: 'Allow'
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
output webAppName string = webAppModule.outputs.webAppName
output webAppHostName string = webAppModule.outputs.defaultHostName
output webAppUrl string = 'https://${webAppModule.outputs.defaultHostName}'
output sqlServerFqdn string = !websiteOnly ? databaseModule!.outputs.serverFqdn : ''
output keyVaultName string = keyVaultModule.outputs.name
output signalRHostName string = signalRModule.outputs.signalRHostName
output appInsightsName string = monitorModule.outputs.appInsightsName
