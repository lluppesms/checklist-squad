// ----------------------------------------------------------------------------------------------------
// Shared Pipeline Parameter File (Azure DevOps + GitHub Actions)
// Uses #{TOKEN}# syntax for pipeline variable substitution
// ----------------------------------------------------------------------------------------------------
using './main.bicep'

param appName = '#{APP_NAME}#'
param environmentCode = '#{ENVCODE}#'
param location = '#{RESOURCE_GROUP_LOCATION}#'
param instanceNumber = '#{INSTANCE_NUMBER}#'
param sqlDatabaseName = '#{SQL_DATABASE_NAME}#'
param sqlAdminLoginUserId = '#{SQLADMIN_LOGIN_USERID}#'
param sqlAdminLoginUserSid = '#{SQLADMIN_LOGIN_USERSID}#'
param sqlAdminLoginTenantId = '#{SQLADMIN_LOGIN_TENANTID}#'
param adminUserId = '#{KEYVAULT_OWNER_USERID}#'
