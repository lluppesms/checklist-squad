// --------------------------------------------------------------------------------
// This BICEP file will create an Azure SQL Server and Database
// --------------------------------------------------------------------------------
param sqlServerName string
param sqlDatabaseName string = 'CheckListDb'
param existingSqlServerName string = ''
param existingSqlServerResourceGroupName string = ''
param location string = resourceGroup().location
param commonTags object = {}
param environmentCode string = 'dev'

param adAdminLoginUserId string = ''
param adAdminLoginUserSid string = ''
param adAdminLoginTenantId string = ''

@description('The Log Analytics workspace ID for diagnostic settings.')
param workspaceId string = ''

// --------------------------------------------------------------------------------
var templateTag = { TemplateFile: '~sqlserver.bicep' }
var tags = union(commonTags, templateTag)

// Tier configuration based on environment
var sqlSkuName = environmentCode == 'prod' ? 'S1' : 'Basic'
var sqlSkuTier = environmentCode == 'prod' ? 'Standard' : 'Basic'
var sqlCapacity = environmentCode == 'prod' ? 20 : 5

var adminDefinition = adAdminLoginUserId == '' ? {} : {
  administratorType: 'ActiveDirectory'
  principalType: 'Group'
  login: adAdminLoginUserId
  sid: adAdminLoginUserSid
  tenantId: adAdminLoginTenantId
  azureADOnlyAuthentication: true
}

var deployNewServer = empty(existingSqlServerName)

// --------------------------------------------------------------------------------
resource existingSqlServerResource 'Microsoft.Sql/servers@2024-05-01-preview' existing = if (!deployNewServer) {
  name: existingSqlServerName
  scope: resourceGroup(existingSqlServerResourceGroupName == '' ? resourceGroup().name : existingSqlServerResourceGroupName)
}
resource existingSqlDBResource 'Microsoft.Sql/servers/databases@2024-05-01-preview' existing = if (!deployNewServer) {
  parent: existingSqlServerResource
  name: sqlDatabaseName
}

resource sqlServerResource 'Microsoft.Sql/servers@2024-05-01-preview' = if (deployNewServer) {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administrators: adminDefinition
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    version: '12.0'
  }
}

resource sqlDBResource 'Microsoft.Sql/servers/databases@2024-05-01-preview' = if (deployNewServer) {
  parent: sqlServerResource
  name: sqlDatabaseName
  location: location
  tags: tags
  sku: {
    name: sqlSkuName
    tier: sqlSkuTier
    capacity: sqlCapacity
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648 // 2 GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
  }
}

// Allow all Azure services to access this server
resource sqlAllowAllAzureIps 'Microsoft.Sql/servers/firewallRules@2024-05-01-preview' = if (deployNewServer) {
  name: 'AllowAllWindowsAzureIps'
  parent: sqlServerResource
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource sqlDBAuditingSettings 'Microsoft.Sql/servers/auditingSettings@2024-05-01-preview' = if (deployNewServer) {
  parent: sqlServerResource
  name: 'default'
  properties: {
    retentionDays: 7
    auditActionsAndGroups: [
      'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
      'FAILED_DATABASE_AUTHENTICATION_GROUP'
      'BATCH_COMPLETED_GROUP'
    ]
    isAzureMonitorTargetEnabled: true
    state: 'Enabled'
  }
}

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (deployNewServer && workspaceId != '') {
  scope: sqlDBResource
  name: '${sqlDatabaseName}-diagnostics'
  properties: {
    workspaceId: workspaceId
    logs: [
      {
        category: 'SQLSecurityAuditEvents'
        enabled: true
      }
    ]
  }
}

// --------------------------------------------------------------------------------
var outputServerName = deployNewServer ? sqlServerResource.name : existingSqlServerResource.name
var outputDatabaseName = deployNewServer ? sqlDBResource.name : existingSqlDBResource.name

output serverName string = outputServerName
output serverFqdn string = '${outputServerName}.database.windows.net'
output databaseName string = outputDatabaseName
output connectionString string = 'Server=tcp:${outputServerName}.database.windows.net,1433;Initial Catalog=${outputDatabaseName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=120;Authentication="Active Directory Default";'
