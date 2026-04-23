# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-04-22 — Schema Isolation Documentation & Risk Analysis

**Schema migration completed:** CheckList app moved from `[dbo]` to dedicated `[CheckList]` schema for shared database hosting.

**Key patterns established:**
- EF Core DbContext specifies schema in `OnModelCreating()`, not in entity classes (keeps entities portable)
- SQL permissions enforce schema isolation, not connection strings (separate deployment principal from app user)
- Seven operational risks identified and mitigation strategies documented: identity collisions (low), cross-schema refs (medium), backup granularity (high), resource contention (medium), migration coordination (high), connection pooling (medium), deployment permissions (medium)
- DACPAC deployment includes pre-deployment script to create `[CheckList]` schema if missing

**Decision:** Comprehensive schema isolation guide published at `docs/schema-isolation.md` for operators setting up shared database. High-risk items (backup granularity, migration coordination) flagged for ongoing coordination with other app teams. Deployment model requires two principals: one for app (read-only CheckList schema), one for DACPAC publish (scoped alter permissions).

### 2025-07-18 — Reference Repo Analysis & Architecture Plan

**Reference repo (lluppesms/dadabase.demo) structure:**
- Root: `.azuredevops/`, `.github/`, `Docs/`, `infra/`, `src/`, solution file at root
- `src/web/Website/` = main Blazor project (`DadABase.Web.csproj`) with folders: API/, Components/, Data/, Helpers/, Models/, Pages/, Repositories/, Shared/, globalUsings.cs
- `src/sql.database/` = DACPAC project using `Microsoft.Build.Sql` SDK v2.0.0, `SqlAzureV12DatabaseSchemaProvider`, folders: dbo/Tables/, dbo/Stored Procedures/, dbo/Views/, Post.Deployment.sql for seed data
- `infra/Bicep/` = main.bicep orchestrator + resourcenames.bicep + modules/ (container, database, function, iam, monitor, security, storage, webapp) + scripts/
- `.github/workflows/` = numbered entrypoint files (1-deploy-bicep.yml, 2.1-*.yml, etc.) + template-*.yml reusable workflows + template-load-config.yml for variable loading
- `.github/actions/` = reusable composite actions, `.github/config/` = config files
- `src/web/Tests/` = test project alongside website project under src/web/

**Old prototype database schema (PRESERVE):**
- Template tables: TemplateSet → TemplateList → TemplateCategory → TemplateAction (master/read-only)
- Check tables: CheckSet → CheckList → CheckCategory → CheckAction (active working copies, created from templates)
- CheckSet has nullable TemplateSetId FK back to source template
- All tables have: audit columns (CreateDateTime, CreateUserName, ChangeDateTime, ChangeUserName), ActiveInd nvarchar(1), SortOrder int
- Sample data: "Lyle's Check Lists" with 7 lists (Trip Prep, Hitching, Arrival, Pre-Depart Interior, Pre-Depart Exterior, Back Home, Maintenance)

**Old prototype code patterns:**
- SignalR hub: `ActionHub : Hub<ITypedHubClient>` with strongly-typed interface
- `ITypedHubClient.BroadcastMessage(string type, string payload)` — single broadcast method
- Repository pattern: one interface + one implementation per entity, `_BaseRepository` base class
- Models use `Newtonsoft.Json` `[JsonProperty]` → NEW BUILD must use `System.Text.Json` `[JsonPropertyName]`
- Namespace was flat `CheckListApp.Data` → NEW BUILD uses hierarchical namespaces per project

**Architecture decisions made:**
- Solution structure: CheckList.AppHost, CheckList.ServiceDefaults, CheckList.Api, CheckList.Web, CheckList.Database, CheckList.Tests
- Namespaces: `CheckList.Api`, `CheckList.Web`, `CheckList.Tests` with sub-namespaces following folder structure
- REST + SignalR hybrid: REST for CRUD, SignalR for real-time push
- Blazor Server (not WASM) for simpler SignalR integration
- EF Core for data access, repository pattern with interfaces
- DACPAC project is schema source of truth (not EF migrations)
- Bicep modules: database, webapp, monitor, security, signalr (new)

**Key file paths:**
- Architecture plan: `.squad/decisions/inbox/dutch-architecture-plan.md`
- Old DB schema: `old-source/Database/GenerateDatabase.sql` (UTF-16 encoded)
- Old seed data: `old-source/Database/LylesCheckList.sql`
- Old models: `old-source/CheckList.Core/Models/Tables/`
- Old hub: `old-source/CheckList.Core/Hub/ActionHub.cs`
- Old repos: `old-source/CheckList.Core/Repository/Interface/` and `Implementation/`

**User preferences confirmed:**
- MSTest for testing
- globalUsings.cs for shared imports
- Scoped .razor.css for component styles
- System.Text.Json (no Newtonsoft)
- File-scoped namespaces, PascalCased
- Docs in Docs/ folder
- IaC in infra/Bicep/
- Pipelines in .github/workflows/ and .azuredevops/pipelines/
