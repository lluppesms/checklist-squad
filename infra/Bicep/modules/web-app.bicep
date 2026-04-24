// --------------------------------------------------------------------------------
// Web App Module
// Creates App Service Plan (conditional) and Web App with full configuration
// Reusable — pass resource names, SKU, app settings from the caller
// --------------------------------------------------------------------------------

@description('Name of the Web App.')
param webSiteName string

@description('Name of the App Service Plan to create (when deploying new).')
param appServicePlanName string

@description('Azure region for resources.')
param location string

@description('Tags to apply to all resources.')
param tags object

@description('Deployment suffix for unique deployment names.')
param deploymentSuffix string

@description('SKU for the App Service Plan.')
@allowed(['B1','B2','S1','S2','S3'])
param webSiteSku string = 'B1'

@description('The kind of web app (linux or windows).')
param webAppKind string = 'linux'

@description('Name of a pre-existing App Service Plan to use instead of creating a new one (empty = create new).')
param servicePlanName string = ''

@description('Resource group of a pre-existing App Service Plan.')
param servicePlanResourceGroupName string = ''

@description('Resource ID of the Application Insights instance.')
param appInsightsResourceId string

@description('Resource ID of the Log Analytics Workspace for diagnostics.')
param logAnalyticsResourceId string

@description('SQL connection string for the web app.')
param webAppConnectionString string = ''

@description('Entra ID tenant ID for app authentication.')
param azureAdTenantId string = ''

@description('Entra ID app registration client ID.')
param azureAdClientId string = ''

@description('Entra ID tenant domain (e.g. myorg.onmicrosoft.com).')
param azureAdDomain string = ''

@allowed(['dev', 'qa', 'prod'])
@description('Environment code — drives ASPNETCORE_ENVIRONMENT setting.')
param environmentCode string = 'dev'

@description('Enable private networking (VNet integration for the web app).')
param enablePrivateNetworking bool = false

@description('Resource ID of the Web App VNet integration subnet (required when enablePrivateNetworking is true).')
param webAppSubnetResourceId string = ''

// --------------------------------------------------------------------------------
// Conditional deploy flag
// --------------------------------------------------------------------------------
var deployNewPlan = empty(servicePlanName)

// --------------------------------------------------------------------------------
// App Service Plan (AVM) — new deployment
// --------------------------------------------------------------------------------
module appServicePlanModule 'br/public:avm/res/web/serverfarm:0.7.0' = if (deployNewPlan) {
  name: 'appServicePlan${deploymentSuffix}'
  params: {
    name: appServicePlanName
    location: location
    tags: tags
    skuName: webSiteSku
    skuCapacity: 1
    kind: webAppKind == 'linux' ? 'linux' : 'app'
    reserved: webAppKind == 'linux'
    zoneRedundant: false
  }
}

// Existing App Service Plan reference (when reusing)
resource existingAppServicePlan 'Microsoft.Web/serverfarms@2024-11-01' existing = if (!deployNewPlan) {
  name: servicePlanName
  scope: resourceGroup(servicePlanResourceGroupName == '' ? resourceGroup().name : servicePlanResourceGroupName)
}

var appServicePlanResourceId = deployNewPlan ? appServicePlanModule!.outputs.resourceId : existingAppServicePlan!.id

// --------------------------------------------------------------------------------
// Web App (AVM)
// --------------------------------------------------------------------------------
module webAppModule 'br/public:avm/res/web/site:0.22.0' = {
  name: 'webapp${deploymentSuffix}'
  params: {
    name: webSiteName
    kind: webAppKind == 'linux' ? 'app,linux' : 'app'
    location: location
    tags: tags
    serverFarmResourceId: appServicePlanResourceId
    virtualNetworkSubnetResourceId: enablePrivateNetworking ? webAppSubnetResourceId : ''
    managedIdentities: {
      systemAssigned: true
    }
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: true
      minTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
      webSocketsEnabled: true
      remoteDebuggingEnabled: false
      linuxFxVersion: webAppKind == 'linux' ? 'DOTNETCORE|10.0' : null
    }
    configs: [
      {
        name: 'appsettings'
        applicationInsightResourceId: appInsightsResourceId
        properties: union({
          ApplicationInsightsAgent_EXTENSION_VERSION: '~3'
          ASPNETCORE_ENVIRONMENT: environmentCode == 'prod' ? 'Production' : 'Development'
          ConnectionStrings__DefaultConnection: webAppConnectionString
          AzureAd__Instance: 'https://login.microsoftonline.com/'
          AzureAd__TenantId: azureAdTenantId
          AzureAd__ClientId: azureAdClientId
          AzureAd__Domain: azureAdDomain
          AzureAd__CallbackPath: '/signin-oidc'
          AzureAd__SignedOutCallbackPath: '/signout-callback-oidc'
        }, enablePrivateNetworking ? {
          WEBSITE_VNET_ROUTE_ALL: '1'
        } : {})
      }
      {
        name: 'logs'
        properties: {
          applicationLogs: {
            fileSystem: {
              level: 'Warning'
            }
          }
          httpLogs: {
            fileSystem: {
              retentionInMb: 40
              enabled: true
            }
          }
          failedRequestsTracing: {
            enabled: true
          }
          detailedErrorMessages: {
            enabled: true
          }
        }
      }
    ]
    diagnosticSettings: [
      {
        workspaceResourceId: logAnalyticsResourceId
        logCategoriesAndGroups: [
          { category: 'AppServiceIPSecAuditLogs' }
          { category: 'AppServiceAuditLogs' }
        ]
      }
    ]
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
@description('Name of the deployed Web App.')
output webAppName string = webAppModule.outputs.name

@description('Default hostname of the Web App.')
output webAppHostName string = webAppModule.outputs.defaultHostname

@description('Default hostname of the Web App (alias for URL building).')
output webAppDefaultHostname string = webAppModule.outputs.defaultHostname

@description('Principal ID of the Web App system-assigned managed identity.')
output systemAssignedMIPrincipalId string = webAppModule.outputs.systemAssignedMIPrincipalId!
