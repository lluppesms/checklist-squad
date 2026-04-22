// --------------------------------------------------------------------------------
// This BICEP file will create a Key Vault with secrets
// FYI: To purge a KV with soft delete enabled: > az keyvault purge --name kvName
// --------------------------------------------------------------------------------
param keyVaultName string
param location string = resourceGroup().location
param commonTags object = {}

@description('Administrators that should have access to administer key vault')
param adminUserObjectIds array = []
@description('Application that should have access to read key vault secrets')
param applicationUserObjectIds array = []

@description('Administrator UserId that should have access to administer key vault')
param keyVaultOwnerUserId string = ''
@description('Ip Address of the KV owner so they can read the vault, such as 254.254.254.254/32')
param keyVaultOwnerIpAddress string = ''

@description('Determines if Azure can deploy certificates from this Key Vault.')
param enabledForDeployment bool = true
@description('Determines if templates can reference secrets from this Key Vault.')
param enabledForTemplateDeployment bool = true
@description('Determines if this Key Vault can be used for Azure Disk Encryption.')
param enabledForDiskEncryption bool = false
@description('Determine if soft delete is enabled on this Key Vault.')
param enableSoftDelete bool = false
@description('Determine if purge protection is enabled on this Key Vault.')
param enablePurgeProtection bool = true
@description('The number of days to retain soft deleted vaults and vault objects.')
param softDeleteRetentionInDays int = 7
@description('Determines if access to the objects granted using RBAC. When true, access policies are ignored.')
param useRBAC bool = false

@allowed(['Enabled','Disabled'])
param publicNetworkAccess string = 'Enabled'
@allowed(['Allow','Deny'])
param allowNetworkAccess string = 'Allow'

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
var adminAccessPolicies = [for adminUser in adminUserObjectIds: {
  objectId: adminUser
  tenantId: subTenantId
  permissions: {
    certificates: [ 'all' ]
    secrets: [ 'all' ]
    keys: [ 'all' ]
  }
}]
var applicationUserPolicies = [for appUser in applicationUserObjectIds: {
  objectId: appUser
  tenantId: subTenantId
  permissions: {
    secrets: [ 'get', 'list' ]
  }
}]

var accessPolicies = union(ownerAccessPolicy, adminAccessPolicies, applicationUserPolicies)

var kvIpRules = keyVaultOwnerIpAddress == '' ? [] : [
  {
    value: keyVaultOwnerIpAddress
  }
]

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
    enableRbacAuthorization: useRBAC
    accessPolicies: accessPolicies
    enabledForDeployment: enabledForDeployment
    enabledForTemplateDeployment: enabledForTemplateDeployment
    enabledForDiskEncryption: enabledForDiskEncryption
    enableSoftDelete: enableSoftDelete
    enablePurgeProtection: enablePurgeProtection
    createMode: 'default'
    softDeleteRetentionInDays: softDeleteRetentionInDays
    publicNetworkAccess: publicNetworkAccess
    networkAcls: {
      defaultAction: allowNetworkAccess
      bypass: 'AzureServices'
      ipRules: kvIpRules
      virtualNetworkRules: []
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

resource keyVaultMetricLogging 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (workspaceId != '') {
  name: '${keyVaultResource.name}-metrics'
  scope: keyVaultResource
  properties: {
    workspaceId: workspaceId
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

// --------------------------------------------------------------------------------
output name string = keyVaultResource.name
output id string = keyVaultResource.id
output vaultUri string = keyVaultResource.properties.vaultUri
