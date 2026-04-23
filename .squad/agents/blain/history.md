# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

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
