# Set up GitHub Secrets

The GitHub workflows in this project require several secrets set at the repository level.

---

## Azure Resource Creation Credentials

You need to set up the Azure Credentials secret in the GitHub Secrets at the Repository level before you do anything else.

See [https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/deploy-github-actions](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/deploy-github-actions) for more info.

To create these secrets, customize and run this command:

``` bash
gh auth login

gh secret set --env dev AZURE_TENANT_ID -b <GUID>
gh secret set --env dev CICD_CLIENT_ID -b <GUID>
gh secret set --env dev AZURE_SUBSCRIPTION_ID -b <yourAzureSubscriptionId>
```

---

## Bicep Configuration Values

These variables and secrets are used by the Bicep templates to configure the resource names that are deployed.  Make sure the APP_NAME variable is unique to your deploy. It will be used as the basis for the website name and for all the other Azure resources, which must be globally unique.

To create these additional secrets and variables, customize and run this command:

``` bash
gh auth login

gh variable set APP_NAME -b 'checklist'
gh variable set RESOURCE_GROUP_LOCATION -b 'centralus'
gh variable set INSTANCE_NUMBER -b 1

gh secret set AZURE_CLIENT_ID -b '<yourADClientId>'
gh secret set AZURE_TENANT_ID -b '<yourTenantId>'
gh secret set AZURE_SUBSCRIPTION_ID -b '<yourSubscriptionId>'

gh secret set SQL_DATABASE_NAME -b 'CheckListDb'
gh secret set SQLADMIN_LOGIN_USERID -b 'youruser@yourdomain.com'
gh secret set SQLADMIN_LOGIN_USERSID -b 'yoursid'
gh secret set SQLADMIN_LOGIN_TENANTID -b 'yourtennant'

gh secret set KEYVAULT_OWNER_USERID -b 'yourAccountSid'

gh secret set AZUREAD_APP_TENANT_ID -b '<yourEntraIdTenantId>'
gh secret set AZUREAD_APP_CLIENT_ID -b '<yourAppRegistrationClientId>'
gh secret set AZUREAD_APP_DOMAIN -b '<yourDomain.onmicrosoft.com>'
```

---

## Summary of Required Secrets and Variables

### Secrets (sensitive values)

| Secret Name | Description |
| --- | --- |
| `AZURE_CLIENT_ID` | Service Principal Application (client) ID for OIDC login |
| `AZURE_TENANT_ID` | Azure AD Tenant ID |
| `AZURE_SUBSCRIPTION_ID` | Azure Subscription ID |
| `SQL_DATABASE_NAME` | Name of the SQL Database |
| `SQLADMIN_LOGIN_USERID` | SQL Admin user email |
| `SQLADMIN_LOGIN_USERSID` | SQL Admin user SID (object ID) |
| `SQLADMIN_LOGIN_TENANTID` | SQL Admin tenant ID |
| `KEYVAULT_OWNER_USERID` | Key Vault administrator principal ID |

### Entra ID App Authentication Secrets

| Secret Name | Description |
| --- | --- |
| `AZUREAD_APP_TENANT_ID` | Entra ID Tenant ID for app user authentication |
| `AZUREAD_APP_CLIENT_ID` | App Registration client ID for user authentication (NOT the deploy service principal) |
| `AZUREAD_APP_DOMAIN` | Entra ID tenant domain (e.g. `myorg.onmicrosoft.com`) |

### Variables (non-sensitive values)

| Variable Name | Description | Example |
| --- | --- | --- |
| `APP_NAME` | Application name used as the basis for all Azure resource names | `checklist` |
| `RESOURCE_GROUP_LOCATION` | Azure region for resource deployment | `centralus` |
| `INSTANCE_NUMBER` | Numeric instance suffix to make resource names unique | `1` |

---

## References

[Deploying ARM Templates with GitHub Actions](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/deploy-github-actions)

[GitHub Secrets CLI](https://cli.github.com/manual/gh_secret_set)
