# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

### 2026-04-21 - Template CRUD API Implementation
- Implemented full CRUD for 4-level template hierarchy (Set → List → Category → Action)
- Added import/export endpoints following dadabase.demo patterns with System.Text.Json attributes
- Created request DTOs for write operations with default values for SortOrder (default: 50)
- Created import/export DTOs with metadata (exportDate, appVersion, itemCount)
- All entities use ActiveInd as nvarchar(1) "Y"/"N" (NOT bool)
- Audit fields (CreateDateTime/CreateUserName/ChangeDateTime/ChangeUserName) set on all create/update operations using UTC timestamps
- Repository methods return DTOs directly for consistency with read operations
- Full import/export includes both templates AND active checklists in single operation
- Cascading deletes configured in DbContext handle hierarchy cleanup automatically
- Default user "System" used for CRUD operations (ready for authentication integration later)
