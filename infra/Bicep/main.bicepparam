// --------------------------------------------------------------------------------
// Shared Pipeline Parameter File (Azure DevOps + GitHub Actions)
// Uses #{TOKEN}# syntax for pipeline variable substitution
// --------------------------------------------------------------------------------
using './main.bicep'

param appName = '#{APP_NAME}#'
param environmentCode = '#{ENVCODE}#'
param location = '#{RESOURCE_GROUP_LOCATION}#'
param instanceNumber = '#{INSTANCE_NUMBER}#'
param websiteOnly = false
param servicePlanName = '#{EXISTING_SERVICEPLAN_NAME}#'
param servicePlanResourceGroupName = '#{EXISTING_SERVICEPLAN_RESOURCE_GROUP_NAME}#'
param webAppKind = 'linux'
param webSiteSku = '#{WEBSITE_SKU}#'
param sqlDatabaseName = '#{SQL_DATABASE_NAME}#'
param existingSqlServerName = '#{EXISTING_SQLSERVER_NAME}#'
param existingSqlServerResourceGroupName = '#{EXISTING_SQLSERVER_RESOURCE_GROUP_NAME}#'
param sqlAdminLoginUserId = '#{SQLADMIN_LOGIN_USERID}#'
param sqlAdminLoginUserSid = '#{SQLADMIN_LOGIN_USERSID}#'
param sqlAdminLoginTenantId = '#{SQLADMIN_LOGIN_TENANTID}#'
param adminUserId = '#{KEYVAULT_OWNER_USERID}#'
param azureAdTenantId = '#{AZUREAD_APP_TENANT_ID}#'
param azureAdClientId = '#{AZUREAD_APP_CLIENT_ID}#'
param azureAdDomain = '#{AZUREAD_APP_DOMAIN}#'
