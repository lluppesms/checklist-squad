// --------------------------------------------------------------------------------
// Main Bicep file that creates all of the Azure Resources for one environment
// Uses Azure Verified Modules (AVM) from the Bicep public registry
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
@allowed(['B1','B2','S1','S2','S3'])
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

@description('The Entra ID (Azure AD) tenant ID for app authentication.')
param azureAdTenantId string = ''

@description('The Entra ID (Azure AD) app registration client ID for app authentication.')
param azureAdClientId string = ''

@description('The Entra ID (Azure AD) tenant domain for app authentication (e.g. myorg.onmicrosoft.com).')
param azureAdDomain string = ''

@description('Address space for the VNet (used when enablePrivateNetworking is true).')
param vnetAddressPrefix string = '10.0.0.0/16'

@description('Subnet prefix for Web App VNet integration.')
param webAppSubnetPrefix string = '10.0.1.0/24'

@description('Subnet prefix for private endpoints.')
param privateEndpointSubnetPrefix string = '10.0.2.0/24'

@description('Enable private networking (VNet, private endpoints, disable public access on backend services).')
param enablePrivateNetworking bool = true

// calculated variables disguised as parameters
param runDateTime string = utcNow()

// --------------------------------------------------------------------------------
var deploymentSuffix = '-${runDateTime}'
var commonTags = {
  LastDeployed: runDateTime
  Application: appName
  Environment: environmentCode
}

// Conditional deploy flags
var deployNewServer = empty(existingSqlServerName)

// SQL connection string (computed from known names, not module outputs)
var sqlServerNameResolved = deployNewServer ? resourceNames.outputs.sqlServerName : existingSqlServerName
var sqlDatabaseHost = environment().suffixes.sqlServerHostname
var webAppConnectionString = !websiteOnly ? 'Server=tcp:${sqlServerNameResolved}.${sqlDatabaseHost},1433;Initial Catalog=${sqlDatabaseName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=120;Authentication="Active Directory Default";' : ''

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
// Monitoring: Log Analytics + Application Insights
// --------------------------------------------------------------------------------
module monitoringModule 'modules/monitoring.bicep' = {
  name: 'monitoring${deploymentSuffix}'
  params: {
    logAnalyticsName: resourceNames.outputs.logAnalyticsWorkspaceName
    appInsightsName: resourceNames.outputs.webSiteAppInsightsName
    location: location
    tags: commonTags
    deploymentSuffix: deploymentSuffix
  }
}

// --------------------------------------------------------------------------------
// Networking: VNet + Private DNS Zones — conditional on enablePrivateNetworking
// --------------------------------------------------------------------------------
module networkingModule 'modules/private-networking.bicep' = if (enablePrivateNetworking) {
  name: 'networking${deploymentSuffix}'
  params: {
    vnetName: resourceNames.outputs.vnetName
    location: location
    tags: commonTags
    vnetAddressPrefix: vnetAddressPrefix
    webAppSubnetPrefix: webAppSubnetPrefix
    privateEndpointSubnetPrefix: privateEndpointSubnetPrefix
    deploymentSuffix: deploymentSuffix
  }
}

// --------------------------------------------------------------------------------
// Database: Azure SQL Server + Database — new deployment
// --------------------------------------------------------------------------------
module sqlServerModule 'modules/sql-server.bicep' = if (deployNewServer && !websiteOnly) {
  name: 'sqlServer${deploymentSuffix}'
  params: {
    sqlServerName: resourceNames.outputs.sqlServerName
    sqlDatabaseName: sqlDatabaseName
    location: location
    tags: commonTags
    deploymentSuffix: deploymentSuffix
    environmentCode: environmentCode
    sqlAdminLoginUserId: sqlAdminLoginUserId
    sqlAdminLoginUserSid: sqlAdminLoginUserSid
    sqlAdminLoginTenantId: sqlAdminLoginTenantId
    logAnalyticsResourceId: monitoringModule.outputs.logAnalyticsResourceId
    enablePrivateNetworking: enablePrivateNetworking
    peSubnetResourceId: enablePrivateNetworking ? networkingModule!.outputs.peSubnetResourceId : ''
    sqlDnsZoneResourceId: enablePrivateNetworking ? networkingModule!.outputs.sqlDnsZoneResourceId : ''
  }
}


// --------------------------------------------------------------------------------
// Web App: App Service Plan + Web App
// --------------------------------------------------------------------------------
module webAppModule 'modules/web-app.bicep' = {
  name: 'webapp${deploymentSuffix}'
  params: {
    webSiteName: resourceNames.outputs.webSiteName
    appServicePlanName: resourceNames.outputs.webSiteAppServicePlanName
    location: location
    tags: commonTags
    deploymentSuffix: deploymentSuffix
    webSiteSku: webSiteSku
    webAppKind: webAppKind
    servicePlanName: servicePlanName
    servicePlanResourceGroupName: servicePlanResourceGroupName
    appInsightsResourceId: monitoringModule.outputs.appInsightsResourceId
    logAnalyticsResourceId: monitoringModule.outputs.logAnalyticsResourceId
    webAppConnectionString: webAppConnectionString
    azureAdTenantId: azureAdTenantId
    azureAdClientId: azureAdClientId
    azureAdDomain: azureAdDomain
    environmentCode: environmentCode
    enablePrivateNetworking: enablePrivateNetworking
    webAppSubnetResourceId: enablePrivateNetworking ? networkingModule!.outputs.webAppSubnetResourceId : ''
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
output webAppName string = webAppModule.outputs.webAppName
output webAppHostName string = webAppModule.outputs.webAppHostName
output webAppUrl string = 'https://${webAppModule.outputs.webAppDefaultHostname}'
output sqlServerFqdn string = !websiteOnly ? '${sqlServerNameResolved}.${sqlDatabaseHost}' : ''
output appInsightsName string = monitoringModule.outputs.appInsightsName
