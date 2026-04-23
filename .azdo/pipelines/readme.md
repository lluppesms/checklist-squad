# Azure DevOps Deployment Template Notes

## 1. Azure DevOps Pipeline Definitions

The following pipelines are available for various deployment scenarios.

### Infrastructure and Application Deployment

- [1-deploy-bicep.yml](1-deploy-bicep.yml): Deploys the main.bicep template to Azure (infrastructure only)
- [2-build-deploy-webapp.yml](2-build-deploy-webapp.yml): Deploys Bicep infrastructure and builds/deploys the .NET Web App to Azure App Service
- [4-build-deploy-dacpac.yml](4-build-deploy-dacpac.yml): Builds and deploys the SQL database schema (DACPAC) to Azure SQL Database

### Code Quality

- [6-pr-scan-build.yml](6-pr-scan-build.yml): Scans and builds code on pull requests

### Subfolders

| Folder | Description |
| --- | --- |
| `steps/` | Reusable step templates shared across pipelines |
| `vars/` | Variable templates for shared configuration |

---

## 2. Deploy Environments

These Azure DevOps YML files are designed to run as multi-stage environment deployments (DEV/QA/PROD). Each Azure DevOps environment can have permissions and approvals defined. For example, DEV can publish on change, while QA and PROD can require approval before changes are made.

---

## 3. Setup Steps

- [Create Azure DevOps Service Connections](https://docs.luppes.com/CreateServiceConnections/)
- [Create Azure DevOps Environments](https://docs.luppes.com/CreateDevOpsEnvironments/)
- Create Azure DevOps Variable Groups (see next section; variables are unique to this project)
- [Create Azure DevOps Pipeline(s)](https://docs.luppes.com/CreateNewPipeline/)
- Run one deployment pipeline (for example, [2-build-deploy-webapp.yml](2-build-deploy-webapp.yml))

---

## 4. Required Variable Group: CheckList.Squad

To create this variable group, customize and run this command in Azure Cloud Shell.

You can also define these variables in the Azure DevOps portal per pipeline, but a variable group is more repeatable and scriptable.

```bash
az login

az pipelines variable-group create \
  --organization=https://dev.azure.com/<yourAzDOOrg>/ \
  --project='<yourAzDOProject>' \
  --name CheckList.Squad \
  --variables \
      APP_NAME='checklist' \
      RESOURCE_GROUP_LOCATION='centralus' \
      INSTANCE_NUMBER='1' \
      AZURE_TENANT_ID='yourTenantId' \
      AZURE_SUBSCRIPTION_ID='yourSubscriptionId' \
      AZURE_CLIENT_ID='yourClientId' \
      SQL_DATABASE_NAME='CheckListDb' \
      SQLADMIN_LOGIN_USERID='youruser@yourdomain.com' \
      SQLADMIN_LOGIN_USERSID='yoursid' \
      SQLADMIN_LOGIN_TENANTID='yourtennant' \
      KEYVAULT_OWNER_USERID='yourAccountSid' \
      EXISTING_SERVICEPLAN_NAME='' \
      EXISTING_SERVICEPLAN_RESOURCE_GROUP_NAME='' \
      EXISTING_SQLSERVER_NAME='' \
      EXISTING_SQLSERVER_RESOURCE_GROUP_NAME='' \
      AZUREAD_APP_TENANT_ID='yourEntraIdTenantId' \
      AZUREAD_APP_CLIENT_ID='yourAppRegistrationClientId' \
      AZUREAD_APP_DOMAIN='yourDomain.onmicrosoft.com'
```

### Variable Descriptions

| Variable | Description | Example |
| --- | --- | --- |
| `APP_NAME` | Application name used as the basis for all Azure resource names | `checklist` |
| `RESOURCE_GROUP_LOCATION` | Azure region for resource deployment | `centralus` |
| `INSTANCE_NUMBER` | Numeric instance suffix to make resource names unique | `1` |
| `AZURE_TENANT_ID` | Azure AD Tenant ID | `<GUID>` |
| `AZURE_SUBSCRIPTION_ID` | Azure Subscription ID | `<GUID>` |
| `AZURE_CLIENT_ID` | Service Principal Application (client) ID | `<GUID>` |
| `SQL_DATABASE_NAME` | Name of the SQL Database | `CheckListDb` |
| `SQLADMIN_LOGIN_USERID` | SQL Admin user email | `user@domain.com` |
| `SQLADMIN_LOGIN_USERSID` | SQL Admin user SID (object ID) | `<GUID>` |
| `SQLADMIN_LOGIN_TENANTID` | SQL Admin tenant ID | `<GUID>` |
| `KEYVAULT_OWNER_USERID` | Key Vault administrator principal ID | `<GUID>` |
| `EXISTING_SERVICEPLAN_NAME` | (Optional) Existing App Service Plan name to reuse | |
| `EXISTING_SERVICEPLAN_RESOURCE_GROUP_NAME` | (Optional) Resource group of existing App Service Plan | |
| `EXISTING_SQLSERVER_NAME` | (Optional) Existing SQL Server name to reuse | |
| `EXISTING_SQLSERVER_RESOURCE_GROUP_NAME` | (Optional) Resource group of existing SQL Server | |
| `AZUREAD_APP_TENANT_ID` | Entra ID Tenant ID for app user authentication | `<GUID>` |
| `AZUREAD_APP_CLIENT_ID` | App Registration client ID for user authentication (NOT the deploy SP) | `<GUID>` |
| `AZUREAD_APP_DOMAIN` | Entra ID tenant domain | `myorg.onmicrosoft.com` |
