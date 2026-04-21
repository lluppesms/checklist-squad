// --------------------------------------------------------------------------------
// Main Bicep file that creates all of the Azure Resources for one environment
// --------------------------------------------------------------------------------
// To deploy this Bicep manually:
//   az login
//   az account set --subscription <subscriptionId>
//   az deployment group create -n manual-$(Get-Date -Format 'yyyyMMddHHmmss') `
//     -g rg-checklist-dev -f ./main.bicep -p ./main.bicepparam
// --------------------------------------------------------------------------------

@description('The name of the application.')
param appName string

@allowed(['dev', 'qa', 'prod'])
@description('The environment code (dev, qa, prod).')
param environmentCode string = 'dev'

@description('The Azure region for resource deployment.')
param location string = resourceGroup().location

@description('The instance number for multiple deployments.')
param instanceNumber string = '1'

@description('The name of the SQL database.')
param sqlDatabaseName string = 'CheckListDb'

@description('The AAD admin login user ID (email).')
param sqlAdminLoginUserId string = ''

@description('The AAD admin login user SID (object ID).')
param sqlAdminLoginUserSid string = ''

@description('The AAD admin login tenant ID.')
param sqlAdminLoginTenantId string = ''

@description('The Key Vault owner user ID (object ID).')
param adminUserId string = ''

// --------------------------------------------------------------------------------
var commonTags = {
  App: appName
  Environment: environmentCode
  Instance: instanceNumber
}

// --------------------------------------------------------------------------------
// Resource Names
// --------------------------------------------------------------------------------
module resourceNames 'resourcenames.bicep' = {
  name: 'resourcenames-${environmentCode}'
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
  name: 'monitor-${environmentCode}'
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
module databaseModule 'modules/database/sqlserver.bicep' = {
  name: 'database-${environmentCode}'
  params: {
    sqlServerName: resourceNames.outputs.sqlServerName
    sqlDatabaseName: sqlDatabaseName
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
  name: 'signalr-${environmentCode}'
  params: {
    signalRServiceName: resourceNames.outputs.signalRServiceName
    location: location
    commonTags: commonTags
    environmentCode: environmentCode
  }
}

// --------------------------------------------------------------------------------
// Web App: App Service Plan + Web App
// --------------------------------------------------------------------------------
module webAppModule 'modules/webapp/webapp.bicep' = {
  name: 'webapp-${environmentCode}'
  params: {
    webSiteName: resourceNames.outputs.webSiteName
    appServicePlanName: resourceNames.outputs.webSiteAppServicePlanName
    location: location
    commonTags: commonTags
    environmentCode: environmentCode
    appInsightsConnectionString: monitorModule.outputs.appInsightsConnectionString
    appInsightsInstrumentationKey: monitorModule.outputs.appInsightsInstrumentationKey
    keyVaultName: resourceNames.outputs.keyVaultName
    workspaceId: monitorModule.outputs.logAnalyticsWorkspaceId
  }
}

// --------------------------------------------------------------------------------
// Security: Key Vault with secrets
// --------------------------------------------------------------------------------
module keyVaultModule 'modules/security/keyvault.bicep' = {
  name: 'keyvault-${environmentCode}'
  params: {
    keyVaultName: resourceNames.outputs.keyVaultName
    location: location
    commonTags: commonTags
    keyVaultOwnerUserId: adminUserId
    applicationUserObjectIds: [webAppModule.outputs.principalId]
    workspaceId: monitorModule.outputs.logAnalyticsWorkspaceId
    sqlConnectionString: databaseModule.outputs.connectionString
    signalRConnectionString: signalRModule.outputs.signalRConnectionString
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
output webAppName string = webAppModule.outputs.webAppName
output webAppHostName string = webAppModule.outputs.defaultHostName
output sqlServerFqdn string = databaseModule.outputs.serverFqdn
output keyVaultName string = keyVaultModule.outputs.name
output signalRHostName string = signalRModule.outputs.signalRHostName
output appInsightsName string = monitorModule.outputs.appInsightsName
