// --------------------------------------------------------------------------------
// Creates Log Analytics Workspace and Application Insights
// --------------------------------------------------------------------------------
param logAnalyticsWorkspaceName string
param appInsightsName string
param location string = resourceGroup().location
param commonTags object = {}

// --------------------------------------------------------------------------------
var templateTag = { TemplateFile: '~monitor.bicep' }
var tags = union(commonTags, templateTag)

// --------------------------------------------------------------------------------
resource logWorkspaceResource 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    workspaceCapping: {
      dailyQuotaGb: 1
    }
  }
}

resource appInsightsResource 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    WorkspaceResourceId: logWorkspaceResource.id
  }
}

// --------------------------------------------------------------------------------
output logAnalyticsWorkspaceId string = logWorkspaceResource.id
output logAnalyticsWorkspaceName string = logWorkspaceResource.name
output appInsightsName string = appInsightsResource.name
output appInsightsInstrumentationKey string = appInsightsResource.properties.InstrumentationKey
output appInsightsConnectionString string = appInsightsResource.properties.ConnectionString
