// --------------------------------------------------------------------------------
// This BICEP file will create an Azure SignalR Service
// --------------------------------------------------------------------------------
param signalRServiceName string
param location string = resourceGroup().location
param commonTags object = {}
param environmentCode string = 'dev'

@description('Allowed origins for CORS. Add the web app URL here.')
param allowedOrigins array = ['*']

// --------------------------------------------------------------------------------
var templateTag = { TemplateFile: '~signalr.bicep' }
var tags = union(commonTags, templateTag)

// Tier configuration based on environment
var skuName = environmentCode == 'prod' ? 'Standard_S1' : 'Free_F1'
var skuTier = environmentCode == 'prod' ? 'Standard' : 'Free'
var skuCapacity = environmentCode == 'prod' ? 1 : 1

// --------------------------------------------------------------------------------
resource signalRResource 'Microsoft.SignalRService/signalR@2024-03-01' = {
  name: signalRServiceName
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: skuTier
    capacity: skuCapacity
  }
  kind: 'SignalR'
  properties: {
    features: [
      {
        flag: 'ServiceMode'
        value: 'Default'
      }
      {
        flag: 'EnableConnectivityLogs'
        value: 'True'
      }
      {
        flag: 'EnableMessagingLogs'
        value: 'True'
      }
    ]
    cors: {
      allowedOrigins: allowedOrigins
    }
    tls: {
      clientCertEnabled: false
    }
    publicNetworkAccess: 'Enabled'
  }
}

// --------------------------------------------------------------------------------
output signalRName string = signalRResource.name
output signalRHostName string = signalRResource.properties.hostName

#disable-next-line outputs-should-not-contain-secrets
output signalRConnectionString string = signalRResource.listKeys().primaryConnectionString
