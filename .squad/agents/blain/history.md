# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2025-07-25: Workflow Rewrite to Match dadabase.demo Golden Code
- **Rewrote 6 workflows** and **created 4 new templates** to match the dadabase.demo repo patterns exactly
- **Key golden code patterns adopted:**
  - `secrets: inherit` everywhere (not individual secret pass-through)
  - `type: environment` for deploy environment inputs (not `type: choice`)
  - `vars.*` for non-sensitive config (APP_NAME, INSTANCE_NUMBER, RESOURCE_GROUP_PREFIX, etc.)
  - Centralized config via `template-load-config.yml` → `.github/actions/load-project-config` → `.github/config/projects.yml`
  - `azure/bicep-deploy@v2` replaces deprecated `azure/arm-deploy` for Bicep deployments
  - `qetza/replacetokens-action@v1` for parameter file token replacement
  - Reusable `./.github/actions/login-action` composite action for Azure auth
  - Action version tags (@v4, @v2, etc.) not SHA-pinned — matches golden code style
  - DACPAC builds on `windows-latest` with MSBuild, not dotnet build
  - Verbose display/debug steps in templates (Display Variables, Display Param File, etc.)
  - Generated resource group/app names via `format()` expressions using `vars.*`
  - Code coverage reporting with `danielpalme/ReportGenerator`, `irongut/CodeCoverageSummary`, and `marocchino/sticky-pull-request-comment`
  - `buildinfo.json` generation in webapp build template
  - What-If analysis support for Bicep deployments
  - Subscription-level deployment support alongside resource group deployments
- **Files changed:** `1-deploy-bicep.yml`, `2-build-deploy-webapp.yml`, `4-build-deploy-dacpac.yml`, `6-pr-scan-build.yml`, `template-bicep-deploy.yml`, `template-webapp-build.yml`
- **Files created:** `template-load-config.yml`, `template-webapp-deploy.yml`, `template-dacpac-build.yml`, `template-dacpac-deploy.yml`
- **Not yet created:** `template-scan-code.yml` (security scanning) and `template-smoke-test.yml` (Playwright tests) — commented out in workflows for future implementation

### 2025-07-25: Full Pipeline Rewrite Pass 2 — Closing Golden Code Gaps
- **Completed all previously deferred work** from the initial golden code alignment
- **GitHub Actions — 5 edits + 3 new files:**
  - Uncommented `scan-code` jobs in workflows 1, 2, and 6 (previously TODO'd)
  - Added `run-smoke-test` job to workflow 2 (bicep-build-deploy-webapp)
  - Created `template-scan-code.yml` — MS DevSecOps + CodeQL GHAS scanning (exact golden copy)
  - Created `template-smoke-test.yml` — Playwright smoke tests (exact golden copy)
  - Created `7-scan-code.yml` — scheduled monthly security scan workflow
  - Updated `template-load-config.yml` with all project types from golden (function, console, mcp, mcpsse, analyzer) — replaces old api-only outputs
- **Azure DevOps — 2 new files + 1 edit:**
  - Created `7-scan-code.yml` — scheduled Wednesday security scan pipeline
  - Created `jobs/playwright-job.yml` — Playwright test job (was referenced by `bicep-and-webapp-stages.yml` but missing)
  - Added containerapp/functionapp/all options to `1-deploy-bicep.yml` deploymentType parameter
- **Key insight:** The `api_*` outputs were removed from template-load-config.yml since Mac is merging API into Web — the golden code's pattern (web, function, console, mcp, sql) is the right model
- **All scan-code and smoke-test functionality is now wired up** and no longer commented out

### 2025-07-25: Azure DevOps Pipeline Rewrite to Match dadabase.demo Golden Code
- **Complete restructure** from flat pipelines to the golden code's 3-tier template hierarchy: Main Pipelines → Stages → Jobs → Steps
- **Rewrote 4 main pipelines** and **created 17 new template files** across jobs/, stages/, steps/, and vars/ directories
- **Key golden code patterns adopted for AzDO:**
  - 3-tier template pattern: main pipelines reference stage templates, which reference job templates, which reference step templates
  - `${{ each environmentName in parameters.environments }}` loop for multi-environment deploys (DEV-QA-PROD)
  - Service connections as hard-coded compile-time variables in `var-service-connections.yml` (not runtime variables)
  - Per-environment variable files (`var-dev.yml`, `var-qa.yml`, `var-prod.yml`) conditionally included at job level
  - `deployment:` jobs for environment gate approvals before actual work jobs
  - `AzureResourceManagerTemplateDeployment@3` for Bicep deploys (not AzureCLI)
  - `qetza.replacetokens.replacetokens-task.replacetokens@5` for parameter file token replacement
  - `AzureRmWebAppDeployment@4` with zip deployment for web app deploys
  - `VSBuild@1` for DACPAC builds on `windows-latest` (not dotnet build)
  - `SqlAzureDacpacDeployment@1` with both `servicePrincipal` and `server` auth types
  - Verbose display/debug steps throughout (Display Variables, Display Files, Display Tree)
  - `buildinfo.json` generation in webapp build
  - Assembly version stamping via `Assembly-Info-NetCore@3`
  - Code coverage with `reportgenerator` and `PublishCodeCoverageResults@2`
  - Bicep output parsing to pipeline variables
  - Summary markdown file generation and artifact publishing
  - Parameter file wiping after deploy for security
  - MS DevSecOps and GHAS scanning support in scan-code-job
  - Variable group `CheckList.Demo` (single group, not per-env groups)
  - Pool set at pipeline level (`pool: vmImage: ubuntu-latest`), except DACPAC uses `windows-latest`
- **Files rewritten:** `1-deploy-bicep.yml`, `2-build-deploy-webapp.yml`, `4-build-deploy-dacpac.yml`, `6-pr-scan-build.yml`, `vars/var-common.yml`, `steps/bicep-deploy-steps.yml`
- **Files created:** `stages/bicep-only-stages.yml`, `stages/bicep-and-webapp-stages.yml`, `stages/dacpac-deploy-stages.yml`, `stages/scan-code-stages.yml`, `stages/webapp-build-stages.yml`, `jobs/bicep-deploy-job.yml`, `jobs/webapp-build-job.yml`, `jobs/webapp-deploy-job.yml`, `jobs/dacpac-build-job.yml`, `jobs/dacpac-deploy-job.yml`, `jobs/scan-code-job.yml`, `steps/dacpac-deploy-steps.yml`, `steps/scan-code-compile-steps.yml`, `vars/var-service-connections.yml`, `vars/var-dev.yml`, `vars/var-qa.yml`, `vars/var-prod.yml`
- **Files removed:** `steps/webapp-build-steps.yml` (replaced by jobs/webapp-build-job.yml)

### 2025-07-26: Bicep Infrastructure Alignment with dadabase.demo Golden Code
- **Rewrote 8 Bicep files** and **created 1 new module** to match golden code patterns exactly
- **Key golden code patterns adopted for Bicep:**
  - Existing App Service Plan support via separate `websiteserviceplan.bicep` module (golden pattern: `existingServicePlanName` + `existingServicePlanResourceGroupName`)
  - Existing SQL Server support via `deployNewServer` conditional pattern (golden: `existingSqlServerName` + `existingSqlServerResourceGroupName`)
  - `websiteOnly` param to skip SQL resources entirely (golden pattern)
  - `webSiteSku` param with `@allowed` constraint for flexible plan sizing (golden: F1-S3 range)
  - `webAppKind` param for linux/windows choice (golden pattern)
  - `runDateTime` param with `utcNow()` and `deploymentSuffix` for deployment tracking (golden pattern)
  - `LastDeployed` tag in commonTags (golden pattern)
  - Webapp module restructured: references existing plan via `existing` keyword, uses `customAppSettings` merge pattern with `union()` (golden pattern)
  - Key Vault enhanced: `adminUserObjectIds` array, `keyVaultOwnerIpAddress` IP rules, `useRBAC` toggle, `enablePurgeProtection`, metric logging (golden features)
  - Resource abbreviations aligned with golden: `appsvc` (was `asp`), `vault` (was `kv`)
  - All conditional deployments wrapped in `if (deployNewServer)` or `if (!websiteOnly)` (golden pattern)
- **Files changed:** `main.bicep`, `main.bicepparam`, `main.parameters.json`, `resourcenames.bicep`, `data/resourceAbbreviations.json`, `modules/database/sqlserver.bicep`, `modules/security/keyvault.bicep`, `modules/webapp/webapp.bicep`
- **Files created:** `modules/webapp/websiteserviceplan.bicep`
- **Files kept as-is:** `modules/monitor/monitor.bicep`, `modules/signalr/signalr.bicep` (project-specific, already correct)
- **Golden features NOT adopted (dadabase-specific, not needed for checklist app):** container modules, function modules, storage modules, OpenAI settings, AAD auth settings, user-assigned managed identity, role assignments module
- **New pipeline variables required:** `EXISTING_SERVICEPLAN_NAME`, `EXISTING_SERVICEPLAN_RESOURCE_GROUP_NAME`, `EXISTING_SQLSERVER_NAME`, `EXISTING_SQLSERVER_RESOURCE_GROUP_NAME`, `WEBSITE_SKU` — set empty strings for fresh deployments
