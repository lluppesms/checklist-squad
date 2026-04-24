// --------------------------------------------------------------------------------
// Monitoring Module
// Creates Log Analytics Workspace and Application Insights
// Reusable — pass resource names, location, and tags from the caller
// --------------------------------------------------------------------------------

@description('Name of the Log Analytics Workspace.')
param logAnalyticsName string

@description('Name of the Application Insights instance.')
param appInsightsName string

@description('Azure region for resources.')
param location string

@description('Tags to apply to all resources.')
param tags object

@description('Deployment suffix for unique deployment names.')
param deploymentSuffix string

// --------------------------------------------------------------------------------
// Log Analytics Workspace (AVM)
// --------------------------------------------------------------------------------
module logAnalyticsModule 'br/public:avm/res/operational-insights/workspace:0.15.0' = {
  name: 'logAnalytics${deploymentSuffix}'
  params: {
    name: logAnalyticsName
    location: location
    tags: tags
    skuName: 'PerGB2018'
    dataRetention: 30
    dailyQuotaGb: '1'
  }
}

// --------------------------------------------------------------------------------
// Application Insights (AVM)
// --------------------------------------------------------------------------------
module appInsightsModule 'br/public:avm/res/insights/component:0.7.1' = {
  name: 'appInsights${deploymentSuffix}'
  params: {
    name: appInsightsName
    location: location
    tags: tags
    kind: 'web'
    applicationType: 'web'
    workspaceResourceId: logAnalyticsModule.outputs.resourceId
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
@description('Resource ID of the Log Analytics Workspace.')
output logAnalyticsResourceId string = logAnalyticsModule.outputs.resourceId

@description('Resource ID of the Application Insights instance.')
output appInsightsResourceId string = appInsightsModule.outputs.resourceId

@description('Name of the Application Insights instance.')
output appInsightsName string = appInsightsModule.outputs.name
