### 2026-04-24: Phase 1 — AVM Migration Complete

**Author:** Blain (DevOps)
**Status:** Implemented

Replaced all 6 local Bicep modules (`infra/Bicep/modules/`) with Azure Verified Modules (AVM) from the public Bicep registry (`br/public:avm/res/...`). This is Phase 1 — public networking preserved; private networking is Phase 2.

**Key changes:**

1. **Azure SignalR Service removed** — The external Azure SignalR Service was provisioned but never consumed (app uses in-process `AddSignalR()`). Removed: module, Key Vault secret (`SignalRConnectionString`), app setting (`Azure__SignalR__ConnectionString`), `signalRServiceName` output from resourcenames.bicep, `signalRHostName` output from main.bicep.

2. **Key Vault switched to RBAC authorization** — `enableRbacAuthorization: true` (now the AVM default). All access policy logic removed. Role assignments:
   - Web app managed identity → `Key Vault Secrets User` (4633458b-17de-408a-b874-0445c86b69e6)
   - Admin user (when provided) → `Key Vault Administrator` (00482a5a-887f-4fb3-b363-3b7fe8e74483)

3. **B1 minimum SKU** — F1 dropped from `webSiteSku` allowed values.

4. **AVM module versions pinned:**
   - operational-insights/workspace: 0.15.0
   - insights/component: 0.7.1
   - sql/server: 0.21.1
   - web/serverfarm: 0.7.0
   - web/site: 0.22.0
   - key-vault/vault: 0.13.3

5. **Existing-resource-reuse patterns preserved** — Conditional deployment for SQL Server and App Service Plan using `deployNewServer`/`deployNewPlan` flags.

**Impact:**
- Pipeline variables unchanged — no new variables needed, no variables removed. `EXISTING_SERVICEPLAN_*` and `EXISTING_SQLSERVER_*` variables still control reuse behavior.
- `bicepconfig.json` created with `br/public` registry alias.
- 6 local module files and their directories deleted.
- The `signalRHostName` output is removed from deployments — any pipeline steps reading this output must be updated.

**Phase 2 scope (not in this change):**
- Private endpoints for SQL, Key Vault, Web App
- VNet integration for App Service
- Temporary firewall rules for CI/CD pipeline access
