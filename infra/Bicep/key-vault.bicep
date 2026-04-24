// --------------------------------------------------------------------------------
// Key Vault Module
// Creates Key Vault with RBAC authorization, secrets, and optional private endpoint
// Reusable — pass resource names, principal IDs, and secrets from the caller
// --------------------------------------------------------------------------------

@description('Name of the Key Vault.')
param keyVaultName string

@description('Azure region for resources.')
param location string

@description('Tags to apply to all resources.')
param tags object

@description('Deployment suffix for unique deployment names.')
param deploymentSuffix string

@description('Resource ID of the Log Analytics Workspace for diagnostics.')
param logAnalyticsResourceId string

@description('Enable private networking (private endpoint, disable public access).')
param enablePrivateNetworking bool = false

@description('Resource ID of the private endpoint subnet (required when enablePrivateNetworking is true).')
param peSubnetResourceId string = ''

@description('Resource ID of the Key Vault Private DNS Zone (required when enablePrivateNetworking is true).')
param kvDnsZoneResourceId string = ''

@description('Principal ID of the Web App managed identity (gets Key Vault Secrets User role).')
param webAppPrincipalId string

@description('Object ID of the admin user (gets Key Vault Administrator role). Empty to skip.')
param adminUserId string = ''

@description('Array of secrets to store in the Key Vault.')
param secrets array = []

@description('Deploy only website infrastructure (controls whether secrets are stored).')
param websiteOnly bool = false

// --------------------------------------------------------------------------------
// Key Vault with RBAC (AVM)
// --------------------------------------------------------------------------------
module keyVaultModule 'br/public:avm/res/key-vault/vault:0.13.3' = {
  name: 'keyvault${deploymentSuffix}'
  params: {
    name: keyVaultName
    location: location
    tags: tags
    enableRbacAuthorization: true
    enablePurgeProtection: true
    softDeleteRetentionInDays: 7
    enableVaultForDeployment: true
    enableVaultForTemplateDeployment: true
    publicNetworkAccess: enablePrivateNetworking ? 'Disabled' : 'Enabled'
    networkAcls: {
      defaultAction: enablePrivateNetworking ? 'Deny' : 'Allow'
      bypass: 'AzureServices'
    }
    privateEndpoints: enablePrivateNetworking ? [
      {
        service: 'vault'
        subnetResourceId: peSubnetResourceId
        privateDnsZoneGroup: {
          privateDnsZoneGroupConfigs: [
            {
              privateDnsZoneResourceId: kvDnsZoneResourceId
            }
          ]
        }
      }
    ] : []
    secrets: !websiteOnly ? secrets : []
    roleAssignments: union(
      [
        {
          principalId: webAppPrincipalId
          roleDefinitionIdOrName: '4633458b-17de-408a-b874-0445c86b69e6' // Key Vault Secrets User
          principalType: 'ServicePrincipal'
        }
      ], adminUserId != '' ? [
        {
          principalId: adminUserId
          roleDefinitionIdOrName: '00482a5a-887f-4fb3-b363-3b7fe8e74483' // Key Vault Administrator
          principalType: 'User'
        }
      ] : []
    )
    diagnosticSettings: [
      {
        workspaceResourceId: logAnalyticsResourceId
        logCategoriesAndGroups: [
          { category: 'AuditEvent' }
        ]
        metricCategories: [
          { category: 'AllMetrics' }
        ]
      }
    ]
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
@description('Name of the deployed Key Vault.')
output keyVaultName string = keyVaultModule.outputs.name
