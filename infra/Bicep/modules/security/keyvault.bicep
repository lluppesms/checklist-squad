// --------------------------------------------------------------------------------
// This BICEP file will create a Key Vault with secrets
// FYI: To purge a KV with soft delete enabled: > az keyvault purge --name kvName
// --------------------------------------------------------------------------------
param keyVaultName string
param location string = resourceGroup().location
param commonTags object = {}

@description('Administrator UserId that should have access to administer key vault')
param keyVaultOwnerUserId string = ''

@description('Object IDs of applications/identities that need secret read access')
param applicationUserObjectIds array = []

@description('The Log Analytics workspace ID for diagnostic settings.')
param workspaceId string = ''

@description('SQL Server connection string to store as a secret')
@secure()
param sqlConnectionString string = ''

@description('SignalR connection string to store as a secret')
@secure()
param signalRConnectionString string = ''

// --------------------------------------------------------------------------------
var templateTag = { TemplateFile: '~keyvault.bicep' }
var tags = union(commonTags, templateTag)
var subTenantId = subscription().tenantId

var ownerAccessPolicy = keyVaultOwnerUserId == '' ? [] : [
  {
    objectId: keyVaultOwnerUserId
    tenantId: subTenantId
    permissions: {
      certificates: [ 'all' ]
      secrets: [ 'all' ]
      keys: [ 'all' ]
    }
  }
]

var applicationUserPolicies = [for appUser in applicationUserObjectIds: {
  objectId: appUser
  tenantId: subTenantId
  permissions: {
    secrets: [ 'get', 'list' ]
  }
}]

var accessPolicies = union(ownerAccessPolicy, applicationUserPolicies)

// --------------------------------------------------------------------------------
resource keyVaultResource 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subTenantId
    enableRbacAuthorization: false
    accessPolicies: accessPolicies
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

// Store SQL connection string as a secret
resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = if (sqlConnectionString != '') {
  parent: keyVaultResource
  name: 'SqlConnectionString'
  properties: {
    value: sqlConnectionString
  }
}

// Store SignalR connection string as a secret
resource signalRConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = if (signalRConnectionString != '') {
  parent: keyVaultResource
  name: 'SignalRConnectionString'
  properties: {
    value: signalRConnectionString
  }
}

resource keyVaultAuditLogging 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (workspaceId != '') {
  name: '${keyVaultResource.name}-auditlogs'
  scope: keyVaultResource
  properties: {
    workspaceId: workspaceId
    logs: [
      {
        category: 'AuditEvent'
        enabled: true
      }
    ]
  }
}

// --------------------------------------------------------------------------------
output name string = keyVaultResource.name
output id string = keyVaultResource.id
output vaultUri string = keyVaultResource.properties.vaultUri
