// --------------------------------------------------------------------------------
// SQL Server Module
// Creates Azure SQL Server with a database, firewall rules, audit settings,
// and optional private endpoint
// Reusable — pass resource names and config from the caller
// --------------------------------------------------------------------------------

@description('Name of the SQL Server.')
param sqlServerName string

@description('Name of the SQL Database.')
param sqlDatabaseName string

@description('Azure region for resources.')
param location string

@description('Tags to apply to all resources.')
param tags object

@description('Deployment suffix for unique deployment names.')
param deploymentSuffix string

@allowed(['dev', 'qa', 'prod'])
@description('Environment code — drives SKU selection.')
param environmentCode string

@description('The AAD admin login user ID (email).')
param sqlAdminLoginUserId string = ''

@description('The AAD admin login user SID (object ID).')
param sqlAdminLoginUserSid string = ''

@description('The AAD admin login tenant ID.')
param sqlAdminLoginTenantId string = ''

@description('Resource ID of the Log Analytics Workspace for diagnostics.')
param logAnalyticsResourceId string

@description('Enable private networking (private endpoint, disable public access).')
param enablePrivateNetworking bool = false

@description('Resource ID of the private endpoint subnet (required when enablePrivateNetworking is true).')
param peSubnetResourceId string = ''

@description('Resource ID of the SQL Private DNS Zone (required when enablePrivateNetworking is true).')
param sqlDnsZoneResourceId string = ''

// --------------------------------------------------------------------------------
// SQL SKU configuration based on environment
// --------------------------------------------------------------------------------
var sqlSkuName = environmentCode == 'prod' ? 'S1' : 'Basic'
var sqlSkuTier = environmentCode == 'prod' ? 'Standard' : 'Basic'
var sqlSkuCapacity = environmentCode == 'prod' ? 20 : 5

// --------------------------------------------------------------------------------
// Azure SQL Server + Database (AVM)
// --------------------------------------------------------------------------------
module sqlServerModule 'br/public:avm/res/sql/server:0.21.1' = {
  name: 'sqlServer${deploymentSuffix}'
  params: {
    name: sqlServerName
    location: location
    tags: union(tags, { TemplateFile: '~sqlserver.bicep', SecurityControl: 'Ignore' })
    minimalTlsVersion: '1.2'
    publicNetworkAccess: enablePrivateNetworking ? 'Disabled' : 'Enabled'
    administrators: sqlAdminLoginUserId != '' ? {
      azureADOnlyAuthentication: true
      login: sqlAdminLoginUserId
      principalType: 'Group'
      sid: sqlAdminLoginUserSid
      tenantId: sqlAdminLoginTenantId
    } : null
    databases: [
      {
        name: sqlDatabaseName
        sku: {
          name: sqlSkuName
          tier: sqlSkuTier
          capacity: sqlSkuCapacity
        }
        collation: 'SQL_Latin1_General_CP1_CI_AS'
        maxSizeBytes: 2147483648 // 2 GB
        availabilityZone: -1
        diagnosticSettings: [
          {
            workspaceResourceId: logAnalyticsResourceId
            logCategoriesAndGroups: [
              { category: 'SQLSecurityAuditEvents' }
            ]
          }
        ]
      }
    ]
    firewallRules: enablePrivateNetworking ? [] : [
      {
        name: 'AllowAllWindowsAzureIps'
        startIpAddress: '0.0.0.0'
        endIpAddress: '0.0.0.0'
      }
    ]
    privateEndpoints: enablePrivateNetworking ? [
      {
        service: 'sqlServer'
        subnetResourceId: peSubnetResourceId
        privateDnsZoneGroup: {
          privateDnsZoneGroupConfigs: [
            {
              privateDnsZoneResourceId: sqlDnsZoneResourceId
            }
          ]
        }
      }
    ] : []
    auditSettings: {
      state: 'Enabled'
      retentionDays: 7
      auditActionsAndGroups: [
        'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
        'FAILED_DATABASE_AUTHENTICATION_GROUP'
        'BATCH_COMPLETED_GROUP'
      ]
      isAzureMonitorTargetEnabled: true
    }
  }
}

// --------------------------------------------------------------------------------
// Outputs
// --------------------------------------------------------------------------------
@description('Name of the deployed SQL Server.')
output sqlServerName string = sqlServerModule.outputs.name
