// --------------------------------------------------------------------------------
// This BICEP file will create an App Service Plan and Web App
// --------------------------------------------------------------------------------
param webSiteName string
param appServicePlanName string
param location string = resourceGroup().location
param commonTags object = {}
param environmentCode string = 'dev'

@description('Application Insights connection string for telemetry.')
param appInsightsConnectionString string = ''
@description('Application Insights instrumentation key.')
param appInsightsInstrumentationKey string = ''
@description('Key Vault name for secret references.')
param keyVaultName string = ''
@description('The Log Analytics workspace ID for diagnostic settings.')
param workspaceId string = ''

// --------------------------------------------------------------------------------
var templateTag = { TemplateFile: '~webapp.bicep' }
var tags = union(commonTags, templateTag)

// Tier configuration based on environment
var skuName = environmentCode == 'prod' ? 'S1' : 'B1'
var skuTier = environmentCode == 'prod' ? 'Standard' : 'Basic'

var kvSecretPrefix = keyVaultName != '' ? '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=' : ''
var kvSecretSuffix = keyVaultName != '' ? ')' : ''

// --------------------------------------------------------------------------------
resource appServicePlanResource 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: skuName
    tier: skuTier
  }
  properties: {
    reserved: true // Required for Linux
  }
}

resource webSiteResource 'Microsoft.Web/sites@2024-04-01' = {
  name: webSiteName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanResource.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      minTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
      alwaysOn: environmentCode == 'prod'
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
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environmentCode == 'prod' ? 'Production' : 'Development'
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: keyVaultName != '' ? '${kvSecretPrefix}SqlConnectionString${kvSecretSuffix}' : ''
        }
        {
          name: 'Azure__SignalR__ConnectionString'
          value: keyVaultName != '' ? '${kvSecretPrefix}SignalRConnectionString${kvSecretSuffix}' : ''
        }
      ]
    }
  }
}

resource webSiteLogsConfig 'Microsoft.Web/sites/config@2024-04-01' = {
  parent: webSiteResource
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
