# Squad Decisions

## Active Decisions

### Workflow Rewrite to Match dadabase.demo Golden Code
**Author:** Blain (DevOps)  
**Date:** 2025-07-25  
**Status:** Implemented

Rewrote all 6 GitHub Actions workflows to match dadabase.demo patterns:
- `secrets: inherit` replaces individual secret pass-through
- `type: environment` for deploy inputs (enables GitHub Environment protection rules)
- `vars.*` for non-sensitive config instead of secrets
- `azure/bicep-deploy@v2` replaces deprecated `azure/arm-deploy`
- `qetza/replacetokens-action@v1` for parameter file token replacement
- Centralized config via `template-load-config.yml` → composite action → `projects.yml`

**Impact:** All team members need to set up GitHub Environment variables instead of using secrets for non-sensitive config.

---

### Azure DevOps Pipeline Architecture — 3-Tier Template Pattern
**Author:** Blain (DevOps)  
**Date:** 2025-07-25  
**Status:** Implemented

Adopted the dadabase.demo 3-tier template hierarchy:
- Single variable group (`CheckList.Demo`) with environment-specific vars in `vars/var-{env}.yml`
- Service connections hard-coded in `var-service-connections.yml`
- `deployment:` jobs as environment gates before work jobs
- DACPAC builds on `windows-latest` using VSBuild (golden code pattern)

**Impact:** Team must create `CheckList.Demo` variable group with: `APP_NAME`, `INSTANCE_NUMBER`, `RESOURCE_GROUP_PREFIX`, `RESOURCE_GROUP_LOCATION`, `SQL_SERVER_NAME_PREFIX`, `SQL_DATABASE_NAME`, `SQLADMIN_LOGIN_USERID`, `SQL_DATABASE_PASSWORD`. Service connections and AzDO environments (`DEV`, `QA`, `PROD`) must be configured.

---

### Bicep Infrastructure Aligned with dadabase.demo Golden Code
**Author:** Blain (DevOps)  
**Date:** 2025-07-26  
**Status:** Implemented

Rewrote all Bicep files to mirror golden code patterns exactly while preserving checklist-specific resources (SignalR, CheckListDb):
- **Existing App Service Plan support** — new `websiteserviceplan.bicep` module handles create-or-reuse logic
- **Existing SQL Server support** — `deployNewServer` conditional on all SQL resources
- **Separated concerns** — App Service Plan is its own module, webapp references it via `existing`
- **Resource abbreviations** now match golden (`appsvc`, `vault`)

**Impact:** Pipelines need new variables: `EXISTING_SERVICEPLAN_NAME`, `EXISTING_SERVICEPLAN_RESOURCE_GROUP_NAME`, `EXISTING_SQLSERVER_NAME`, `EXISTING_SQLSERVER_RESOURCE_GROUP_NAME`, `WEBSITE_SKU`. Set to empty strings for fresh deployments.

---

### Align All CI/CD Pipelines with dadabase.demo Golden Code
**Author:** Blain (DevOps)  
**Date:** 2025-07-25  
**Status:** Implemented

Completed golden code alignment:
1. Uncommented all scan-code jobs
2. Created 3 missing GHA templates (scan-code, smoke-test, 7-scan-code)
3. Created 2 missing AzDO files (7-scan-code, playwright-job)
4. Updated template-load-config.yml to match golden's full project type catalog
5. Added all deployment types to AzDO 1-deploy-bicep.yml

**Files Changed:** 10 files (5 modified, 5 created) across both GHA and AzDO. Security scanning now runs on PRs, deploys, and monthly schedule. Smoke tests (Playwright) wired into deploy workflow.

---

### Merge CheckList.Api into CheckList.Web (Single-App Architecture)
**Author:** Mac (Backend Dev)  
**Date:** 2026-04-21  
**Status:** Implemented

Merged separate API project into Blazor Server app as single unified deployment:
- Blazor pages now call repositories directly via `CheckListService` instead of HTTP
- API controllers preserved for future external consumers
- SignalR hub hosted locally (no cross-service WebSocket connections)
- Aspire AppHost simplified to one project reference

**Trade-offs:** Simpler deployment and no HTTP overhead vs. loss of independent API scaling (not needed for this app's scale).

---

### Selective List Activation (API & Frontend)
**Author:** Mac (Backend Dev), Poncho (Frontend Dev)  
**Date:** 2026-04-21  
**Status:** Implemented

Added optional selective list activation to `/api/checklists/activate/{templateSetId}` endpoint:
- Added optional `SelectedListIds` parameter to `ActivateCheckSetRequest`
- Backend validates selected IDs exist in template (fails fast with BadRequest)
- Frontend shows `ListSelectionDialog` with all lists pre-checked (users can deselect unwanted)
- Full-screen dialog on mobile, checkboxes sized for easy tapping (1.25rem)
- Added "Select All" / "Deselect All" toggle and live selection counter

**Backward Compatible:** When `SelectedListIds` is null or empty, all lists are activated (preserves existing behavior).

---

### Test Coverage Standard
**Status:** Implemented (2026-04-22)

Unit test coverage baseline established:
- Target: 90%+ line coverage on testable code
- Achieved: 94.5% line / 92% branch coverage (144 tests)
- Testable code excludes: Blazor UI (`**/Components/**`), Program.cs, Extensions.cs
- One test file per class under Services/, Repositories/, Controllers/, Hubs/, Models/
- Shared `DbContextHelper` provides in-memory DbContext with seed data
- Test frameworks: MSTest 4.0.1, Moq 4.20.72, EF Core InMemory 10.0.6

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
