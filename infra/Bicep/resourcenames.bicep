// --------------------------------------------------------------------------------
// Bicep file that builds all the resource names used by other Bicep templates
// --------------------------------------------------------------------------------
param appName string = ''
param environmentCode string = 'dev'
param instanceNumber string = '1'

// --------------------------------------------------------------------------------
var sanitizedEnvironment = toLower(environmentCode)
var sanitizedAppNameWithDashes = replace(replace(toLower(appName), ' ', ''), '_', '')
var sanitizedAppInstanceNameWithDashes = replace(replace(toLower('${appName}${instanceNumber}'), ' ', ''), '_', '')
var sanitizedAppNameInstance = replace(replace(replace(toLower('${appName}${instanceNumber}'), ' ', ''), '_', ''), '-', '')

// --------------------------------------------------------------------------------
var resourceAbbreviations = loadJsonContent('./data/resourceAbbreviations.json')

// --------------------------------------------------------------------------------
var webSiteName = environmentCode == 'prod' ? toLower('${sanitizedAppNameWithDashes}') : toLower('${sanitizedAppInstanceNameWithDashes}-${sanitizedEnvironment}')

// --------------------------------------------------------------------------------
output webSiteName string = webSiteName
output webSiteAppServicePlanName string = '${webSiteName}-${resourceAbbreviations.appServicePlanSuffix}'
output webSiteAppInsightsName string = '${webSiteName}-${resourceAbbreviations.appInsightsSuffix}'
output sqlServerName string = toLower('${sanitizedAppNameInstance}${resourceAbbreviations.sqlAbbreviation}${sanitizedEnvironment}')
output logAnalyticsWorkspaceName string = toLower('${sanitizedAppInstanceNameWithDashes}-${sanitizedEnvironment}-${resourceAbbreviations.logWorkspaceSuffix}')
output signalRServiceName string = toLower('${sanitizedAppInstanceNameWithDashes}-${resourceAbbreviations.signalRSuffix}-${sanitizedEnvironment}')
output userAssignedIdentityName string = toLower('${sanitizedAppNameInstance}-app-${resourceAbbreviations.managedIdentity}')

// Key Vault names are capped at 24 characters
output keyVaultName string = take('${sanitizedAppNameInstance}${resourceAbbreviations.keyVaultAbbreviation}${sanitizedEnvironment}', 24)
