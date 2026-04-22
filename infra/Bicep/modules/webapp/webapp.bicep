// --------------------------------------------------------------------------------
// This BICEP file will create an Azure Website
// --------------------------------------------------------------------------------
param webSiteName string = ''
param location string = resourceGroup().location
param environmentCode string = 'dev'
param commonTags object = {}

@description('The workspace to store audit logs.')
param workspaceId string = ''

@description('The Name of the service plan to deploy into.')
param appServicePlanName string
param appServicePlanResourceGroupName string = resourceGroup().name
param webAppKind string = 'linux'

@description('Application Insights connection string for telemetry.')
param appInsightsConnectionString string = ''
@description('Application Insights instrumentation key.')
param appInsightsInstrumentationKey string = ''

@description('Custom application settings to merge with the base settings.')
param customAppSettings object = {}

// --------------------------------------------------------------------------------
var templateTag = { TemplateFile: '~webapp.bicep' }
var tags = union(commonTags, templateTag)

var linuxFxVersion = webAppKind == 'linux' ? 'DOTNETCORE|10.0' : ''

// Base app settings that are always applied
var baseAppSettings = {
  APPLICATIONINSIGHTS_CONNECTION_STRING: appInsightsConnectionString
  APPINSIGHTS_INSTRUMENTATIONKEY: appInsightsInstrumentationKey
  ApplicationInsightsAgent_EXTENSION_VERSION: '~3'
}

// Merge base settings with custom settings
var mergedAppSettings = union(baseAppSettings, customAppSettings)

// --------------------------------------------------------------------------------
resource appServiceResource 'Microsoft.Web/serverfarms@2024-11-01' existing = {
  name: appServicePlanName
  scope: resourceGroup(appServicePlanResourceGroupName)
}

resource webSiteResource 'Microsoft.Web/sites@2024-11-01' = {
  name: webSiteName
  location: location
  tags: tags
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServiceResource.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      minTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
      alwaysOn: true
      remoteDebuggingEnabled: false
      webSocketsEnabled: true // Required for SignalR
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsightsInstrumentationKey
        }
      ]
    }
  }
}

// App Settings Configuration - merges base settings with custom app settings
resource webSiteAppSettingsConfig 'Microsoft.Web/sites/config@2024-11-01' = {
  parent: webSiteResource
  name: 'appsettings'
  properties: mergedAppSettings
}

resource webSiteLogsConfig 'Microsoft.Web/sites/config@2024-11-01' = {
  parent: webSiteResource
  name: 'logs'
  dependsOn: [webSiteAppSettingsConfig]
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

resource webSiteAuditLogging 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (workspaceId != '') {
  name: '${webSiteResource.name}-auditlogs'
  scope: webSiteResource
  properties: {
    workspaceId: workspaceId
    logs: [
      {
        category: 'AppServiceIPSecAuditLogs'
        enabled: true
      }
      {
        category: 'AppServiceAuditLogs'
        enabled: true
      }
    ]
  }
}

// --------------------------------------------------------------------------------
output webAppName string = webSiteResource.name
output defaultHostName string = webSiteResource.properties.defaultHostName
output principalId string = webSiteResource.identity.principalId
