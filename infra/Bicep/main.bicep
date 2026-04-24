// --------------------------------------------------------------------------------
// Main Bicep file that creates all of the Azure Resources for one environment
// Uses Azure Verified Modules (AVM) from the Bicep public registry
// --------------------------------------------------------------------------------
// To deploy this Bicep manually:
//   az login
//   az account set --subscription <subscriptionId>
//   az deployment group create -n "manual-$(Get-Date -Format 'yyyyMMdd-HHmmss')" `
//     --resource-group rg-checklist-dev --template-file 'main.bicep' --parameters appName=xxx environmentCode=dev adminUserId=xxxxxxxx-xxxx-xxxx
// --------------------------------------------------------------------------------
param appName string

@allowed(['dev', 'qa', 'prod'])
@description('The environment code (dev, qa, prod).')
param environmentCode string = 'dev'

@description('The Azure region for resource deployment.')
param location string = resourceGroup().location

@description('The instance number for multiple deployments.')
param instanceNumber string = '1'

@description('Deploy only website infrastructure (skip SQL resources).')
param websiteOnly bool = false

@description('Name of a pre-existing App Service Plan to use instead of creating a new one.')
param servicePlanName string = ''

@description('Resource group of a pre-existing App Service Plan.')
param servicePlanResourceGroupName string = ''

@description('The kind of web app (linux or windows).')
param webAppKind string = 'linux'

@description('The SKU for the App Service Plan when creating a new one.')
@allowed(['B1','B2','S1','S2','S3'])
param webSiteSku string = 'B1'

@description('The name of the SQL database.')
param sqlDatabaseName string = 'CheckListDb'

@description('Name of an existing SQL Server to use instead of creating a new one.')
param existingSqlServerName string = ''

@description('Resource group of an existing SQL Server.')
param existingSqlServerResourceGroupName string = ''

@description('The AAD admin login user ID (email).')
param sqlAdminLoginUserId string = ''

@description('The AAD admin login user SID (object ID).')
param sqlAdminLoginUserSid string = ''

@description('The AAD admin login tenant ID.')
param sqlAdminLoginTenantId string = ''

@description('The Key Vault owner user ID (object ID).')
param adminUserId string = ''

@description('The Entra ID (Azure AD) tenant ID for app authentication.')
param azureAdTenantId string = ''

@description('The Entra ID (Azure AD) app registration client ID for app authentication.')
param azureAdClientId string = ''

@description('The Entra ID (Azure AD) tenant domain for app authentication (e.g. myorg.onmicrosoft.com).')
param azureAdDomain string = ''

// calculated variables disguised as parameters
param runDateTime string = utcNow()

// --------------------------------------------------------------------------------
var deploymentSuffix = '-${runDateTime}'
var commonTags = {
  LastDeployed: runDateTime
  Application: appName
  Environment: environmentCode
}

// SQL tier configuration based on environment
var sqlSkuName = environmentCode == 'prod' ? 'S1' : 'Basic'
var sqlSkuTier = environmentCode == 'prod' ? 'Standard' : 'Basic'
var sqlSkuCapacity = environmentCode == 'prod' ? 20 : 5

// Conditional deploy flags
var deployNewServer = empty(existingSqlServerName)
var deployNewPlan = empty(servicePlanName)

// SQL connection string (computed from known names, not module outputs)
var sqlServerNameResolved = deployNewServer ? resourceNames.outputs.sqlServerName : existingSqlServerName
var webAppConnectionString = !websiteOnly ? 'Server=tcp:${sqlServerNameResolved}.database.windows.net,1433;Initial Catalog=${sqlDatabaseName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=120;Authentication="Active Directory Default";' : ''

// --------------------------------------------------------------------------------
// Resource Names
// --------------------------------------------------------------------------------
module resourceNames 'resourcenames.bicep' = {
  name: 'resourcenames${deploymentSuffix}'
  params: {
    appName: appName
    environmentCode: environmentCode
    instanceNumber: instanceNumber
  }
}

// --------------------------------------------------------------------------------
// Monitoring: Log Analytics Workspace (AVM)
// --------------------------------------------------------------------------------
module logAnalyticsModule 'br/public:avm/res/operational-insights/workspace:0.15.0' = {
  name: 'logAnalytics${deploymentSuffix}'
  params: {
    name: resourceNames.outputs.logAnalyticsWorkspaceName
    location: location
    tags: commonTags
    skuName: 'PerGB2018'
    dataRetention: 30
    dailyQuotaGb: '1'
  }
}

// --------------------------------------------------------------------------------
// Monitoring: Application Insights (AVM)
// --------------------------------------------------------------------------------
module appInsightsModule 'br/public:avm/res/insights/component:0.7.1' = {
  name: 'appInsights${deploymentSuffix}'
  params: {
    name: resourceNames.outputs.webSiteAppInsightsName
    location: location
    tags: commonTags
    kind: 'web'
    applicationType: 'web'
    workspaceResourceId: logAnalyticsModule.outputs.resourceId
  }
}

// --------------------------------------------------------------------------------
// Database: Azure SQL Server + Database (AVM) — new deployment
// --------------------------------------------------------------------------------
module sqlServerModule 'br/public:avm/res/sql/server:0.21.1' = if (deployNewServer && !websiteOnly) {
  name: 'sqlServer${deploymentSuffix}'
  params: {
    name: resourceNames.outputs.sqlServerName
    location: location
    tags: union(commonTags, { TemplateFile: '~sqlserver.bicep', SecurityControl: 'Ignore' })
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
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
            workspaceResourceId: logAnalyticsModule.outputs.resourceId
            logCategoriesAndGroups: [
              { category: 'SQLSecurityAuditEvents' }
            ]
          }
        ]
      }
    ]
    firewallRules: [
      {
        name: 'AllowAllWindowsAzureIps'
        startIpAddress: '0.0.0.0'
        endIpAddress: '0.0.0.0'
      }
    ]
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
// App Service Plan (AVM) — new deployment
// --------------------------------------------------------------------------------
module appServicePlanModule 'br/public:avm/res/web/serverfarm:0.7.0' = if (deployNewPlan) {
  name: 'appServicePlan${deploymentSuffix}'
  params: {
    name: resourceNames.outputs.webSiteAppServicePlanName
    location: location
    tags: commonTags
    skuName: webSiteSku
    skuCapacity: 1
    kind: webAppKind == 'linux' ? 'linux' : 'app'
    reserved: webAppKind == 'linux'
    zoneRedundant: false
  }
}

// Existing App Service Plan reference (when reusing)
resource existingAppServicePlan 'Microsoft.Web/serverfarms@2024-11-01' existing = if (!deployNewPlan) {
  name: servicePlanName
  scope: resourceGroup(servicePlanResourceGroupName == '' ? resourceGroup().name : servicePlanResourceGroupName)
}

var appServicePlanResourceId = deployNewPlan ? appServicePlanModule!.outputs.resourceId : existingAppServicePlan!.id

// --------------------------------------------------------------------------------
// Web App (AVM)
// --------------------------------------------------------------------------------
module webAppModule 'br/public:avm/res/web/site:0.22.0' = {
  name: 'webapp${deploymentSuffix}'
  params: {
    name: resourceNames.outputs.webSiteName
    kind: webAppKind == 'linux' ? 'app,linux' : 'app'
    location: location
    tags: commonTags
    serverFarmResourceId: appServicePlanResourceId
    managedIdentities: {
      systemAssigned: true
    }
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: true
      minTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
      webSocketsEnabled: true
      remoteDebuggingEnabled: false
      linuxFxVersion: webAppKind == 'linux' ? 'DOTNETCORE|10.0' : null
    }
    configs: [
      {
        name: 'appsettings'
        applicationInsightResourceId: appInsightsModule.outputs.resourceId
        properties: {
          ApplicationInsightsAgent_EXTENSION_VERSION: '~3'
          ASPNETCORE_ENVIRONMENT: environmentCode == 'prod' ? 'Production' : 'Development'
          ConnectionStrings__DefaultConnection: webAppConnectionString
          AzureAd__Instance: 'https://login.microsoftonline.com/'
          AzureAd__TenantId: azureAdTenantId
          AzureAd__ClientId: azureAdClientId
          AzureAd__Domain: azureAdDomain
          AzureAd__CallbackPath: '/signin-oidc'
          AzureAd__SignedOutCallbackPath: '/signout-callback-oidc'
        }
      }
      {
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
    ]
    diagnosticSettings: [
      {
        workspaceResourceId: logAnalyticsModule.outputs.resourceId
        logCategoriesAndGroups: [
          { category: 'AppServiceIPSecAuditLogs' }
          { category: 'AppServiceAuditLogs' }
        ]
      }
    ]
  }
}

// --------------------------------------------------------------------------------
// Security: Key Vault with RBAC (AVM)
// --------------------------------------------------------------------------------
module keyVaultModule 'br/public:avm/res/key-vault/vault:0.13.3' = {
  name: 'keyvault${deploymentSuffix}'
  params: {
    name: resourceNames.outputs.keyVaultName
    location: location
    tags: commonTags
    enableRbacAuthorization: true
    enablePurgeProtection: true
    softDeleteRetentionInDays: 7
    enableVaultForDeployment: true
    enableVaultForTemplateDeployment: true
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    secrets: !websiteOnly ? [
      {
        name: 'SqlConnectionString'
        value: webAppConnectionString
      }
    ] : []
    roleAssignments: union(
      [
        {
          principalId: webAppModule.outputs.systemAssignedMIPrincipalId!
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
        workspaceResourceId: logAnalyticsModule.outputs.resourceId
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
output webAppName string = webAppModule.outputs.name
output webAppHostName string = webAppModule.outputs.defaultHostname
output webAppUrl string = 'https://${webAppModule.outputs.defaultHostname}'
output sqlServerFqdn string = !websiteOnly ? '${sqlServerNameResolved}.database.windows.net' : ''
output keyVaultName string = keyVaultModule.outputs.name
output appInsightsName string = appInsightsModule.outputs.name
