# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-07-17: Phase 3 — Extract Reusable Bicep Modules
- **Extracted 4 modules** from main.bicep: `monitoring.bicep`, `sql-server.bicep`, `web-app.bicep`, `key-vault.bicep`
- **main.bicep is now ~226 lines** — a clean orchestration file calling 6 modules (resourcenames, monitoring, private-networking, sql-server, web-app, key-vault)
- **Module design:** Each module is self-contained with `@description()` on all params, typed outputs, and reusable across projects
- **What moved into modules:**
  - SQL SKU vars (sqlSkuName/Tier/Capacity) → sql-server.bicep
  - deployNewPlan conditional + existing plan reference → web-app.bicep
  - All AVM module calls with their configs (firewall rules, audit settings, siteConfig, RBAC, diagnostics, private endpoints)
- **What stayed in main.bicep:** All params, resourceNames module, conditional deploy flags (deployNewServer), connection string computation, networking module call, cross-module wiring, outputs
- **Cross-module wiring pattern:** monitoring outputs feed into sql-server, web-app, and key-vault; networking outputs conditionally feed PE/subnet IDs; web-app principal ID feeds into key-vault
- **AVM module versions unchanged** — same versions as Phase 1
- **Build status:** `az bicep build` passes with zero errors. Same pre-existing warnings (BCP081, hardcoded URLs)

### 2026-07-17: Phase 2 — Private Networking(VNet + Private Endpoints + DNS Zones)
- **Added conditional private networking** to AVM Bicep templates, gated behind `enablePrivateNetworking` param (default: `true`)
- **New AVM modules:**
  - `br/public:avm/res/network/virtual-network:0.8.1` — VNet with two subnets
  - `br/public:avm/res/network/private-dns-zone:0.8.1` — SQL and Key Vault DNS zones
- **VNet layout:**
  - `snet-webapp` (`10.0.1.0/24`) — delegated to `Microsoft.Web/serverFarms` for Web App VNet integration
  - `snet-pe` (`10.0.2.0/24`) — no delegation, hosts private endpoints for SQL + Key Vault
- **Private DNS Zones:** `privatelink.database.windows.net` and `privatelink.vaultcore.azure.net`, each linked to the VNet
- **SQL Server changes:** `publicNetworkAccess: 'Disabled'`, `AllowAllWindowsAzureIps` firewall rule removed, inline PE on snet-pe with SQL DNS zone. SQL PE only created when `deployNewServer && enablePrivateNetworking` (can't add PE to external SQL server).
- **Key Vault changes:** `publicNetworkAccess: 'Disabled'`, `networkAcls.defaultAction: 'Deny'`, inline PE on snet-pe with KV DNS zone
- **Web App changes:** `virtualNetworkSubnetResourceId` set to snet-webapp, `WEBSITE_VNET_ROUTE_ALL=1` app setting added
- **AVM VNet module key difference:** Property is `virtualNetworkSubnetResourceId` (not `virtualNetworkSubnetId`) on web/site module
- **All networking resources are conditional** — when `enablePrivateNetworking=false`, deployment behaves identically to Phase 1 (public access)
- **New params:** `vnetAddressPrefix`, `webAppSubnetPrefix`, `privateEndpointSubnetPrefix`, `enablePrivateNetworking`
- **Pipeline variables needed:** `VNET_ADDRESS_PREFIX`, `WEBAPP_SUBNET_PREFIX`, `PE_SUBNET_PREFIX`, `ENABLE_PRIVATE_NETWORKING`
- **Files changed:** `main.bicep` (+104 lines), `resourcenames.bicep` (vnetName output), `main.bicepparam` (4 new tokens)
- **Build status:** `az bicep build` passes with zero errors. Same pre-existing warnings as Phase 1.

### 2026-04-24: Phase 1 — AVM Migration (Replace Local Bicep Modules)
- **Replaced all 6 local Bicep modules** with Azure Verified Modules (AVM) from the public Bicep registry
- **AVM module versions used:**
  - `br/public:avm/res/operational-insights/workspace:0.15.0` — Log Analytics
  - `br/public:avm/res/insights/component:0.7.1` — App Insights
  - `br/public:avm/res/sql/server:0.21.1` — SQL Server (includes databases, firewallRules, auditSettings as child params)
  - `br/public:avm/res/web/serverfarm:0.7.0` — App Service Plan
  - `br/public:avm/res/web/site:0.22.0` — Web App (uses `configs` array with discriminated union for appsettings/logs)
  - `br/public:avm/res/key-vault/vault:0.13.3` — Key Vault (RBAC mode, role assignments, secrets, diagnostics)
- **Removed Azure SignalR Service entirely** — App uses in-process `AddSignalR()`, external SignalR was provisioned but never consumed. Removed module, KV secret, app setting, output.
- **Key Vault switched to RBAC** — `enableRbacAuthorization: true` (AVM default). Web app managed identity gets `Key Vault Secrets User` role (4633458b-17de-408a-b874-0445c86b69e6). Admin user gets `Key Vault Administrator` (00482a5a-887f-4fb3-b363-3b7fe8e74483). Access policies dropped.
- **B1 minimum SKU** — Dropped F1 from allowed values per decision.
- **Existing-resource-reuse patterns preserved** — Conditional `deployNewServer` / `deployNewPlan` flags. Existing resources referenced via `existing` keyword. Connection string computed from known names (no module output dependency for existing servers).
- **AVM module key differences from hand-rolled modules:**
  - Key Vault: `enableVaultForDeployment` not `enabledForDeployment`; `enableVaultForTemplateDeployment` not `enabledForTemplateDeployment`
  - Web/Site: App settings via `configs` array (discriminated union with `name: 'appsettings'`), not `appSettingsKeyValuePairs`; `applicationInsightResourceId` auto-wires App Insights
  - Web/Serverfarm: `skuName` not `sku.name`; `kind: 'linux'` sets `reserved: true` automatically
  - SQL/Server: `databases` array includes `availabilityZone: -1`, `sku` object; `auditSettings` is top-level param; `administrators` type has required fields (azureADOnlyAuthentication, login, principalType, sid)
  - Operational-Insights: `dailyQuotaGb` is string type (not int)
- **Files changed:** `main.bicep` (rewritten), `resourcenames.bicep` (SignalR output removed), `bicepconfig.json` (created)
- **Files deleted:** 6 module files + 5 module directories under `infra/Bicep/modules/`
- **Build status:** `az bicep build` passes with zero errors. Warnings are all non-blocking (BCP081 API type availability, hardcoded URL patterns carried from original code)

### 2026-04-22: SQL Database Schema Restructure from [dbo] to [CheckList]
- **Restructured entire SQL Database Project** to use custom `[CheckList]` schema instead of default `[dbo]`
- **Migration approach:** Created parallel `CheckList/` folder structure alongside existing `dbo/` folder (dbo files left for reference/rollback)
- **Files migrated:** 12 tables, 1 stored procedure, Pre.Deployment.sql (13 refs), Post.Deployment.sql (23 refs) — total 133 `[dbo]` → `[CheckList]` replacements across 15 files
- **Schema creation safety:** Added idempotent schema check to Pre.Deployment.sql (checks `sys.schemas` before creating) plus standalone `CheckList.Schema.sql` file
- **Project file update:** `.sqlproj` now references `CheckList\Pre.Deployment.sql` and `CheckList\Post.Deployment.sql` instead of `dbo\` paths
- **Verification:** Zero `[dbo]` references in CheckList folder (excluding `AUTHORIZATION [dbo]` in schema creation), 146 `[CheckList]` references total
- **Directory structure:** `CheckList/Tables/`, `CheckList/Stored Procedures/`, `CheckList/Views/` — mirrors old `dbo/` hierarchy
- **Files changed:** `CheckList.Database.sqlproj`, all 16 .sql files in new CheckList/ folder
- **Key insight:** Pre-deployment wipe logic preserved (CheckSet.OwnerId nullable→NOT NULL migration), seed data integrity maintained through schema rename
- **Deployment impact:** First DACPAC publish will create `[CheckList]` schema automatically via Pre.Deployment.sql safety check — no manual intervention needed
- **Coordinated with Mac's EF Core schema migration** — all 12 DbContext entities now reference `[CheckList]` schema
- **Status:** COMPLETE — All 209 tests passing, DACPAC builds successfully

## Archived Learnings (2025)

### 2025-07-25 to 2025-07-26 — Golden Code Alignment Initiative
- **Rewrote 6 GitHub Actions workflows** and **created 7 new GHA/AzDO files** to match dadabase.demo patterns
- **Complete Azure DevOps pipeline restructure** from flat to 3-tier template hierarchy (Main → Stages → Jobs → Steps), **created 17 new AzDO template files**
- **Rewrote 8 Bicep files** and **created 1 new module** (websiteserviceplan.bicep) to support existing App Service Plans and SQL Servers
- **Key patterns:** `secrets: inherit`, `type: environment`, `vars.*` for config, `azure/bicep-deploy@v2`, 3-tier template hierarchy, idempotent SQL pre-deployment scripts
- **SQL automation:** Added `5-run-sql-script.yml` + `template-run-sql.yml` for post-DACPAC managed identity grants via Azure AD tokens and Invoke-Sqlcmd
- **Pipeline variables required:** `EXISTING_SERVICEPLAN_NAME`, `EXISTING_SERVICEPLAN_RESOURCE_GROUP_NAME`, `EXISTING_SQLSERVER_NAME`, `EXISTING_SQLSERVER_RESOURCE_GROUP_NAME`, `WEBSITE_SKU`, `APP_NAME`
