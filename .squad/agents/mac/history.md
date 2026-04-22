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

### 2026-04-21 - Selective List Activation Feature
- Added optional `SelectedListIds` parameter to `ActivateCheckSetRequest` (backward compatible - null/empty activates all lists)
- Updated repository and controller layers to support selective list activation
- When specific list IDs provided, only matching template lists are copied to the active checklist
- Returns ArgumentException (BadRequest) if provided list IDs don't match any lists in the template
- Maintains full backward compatibility - existing API calls without SelectedListIds continue to work unchanged
- Enables use cases like skipping yearly maintenance lists on daily activation runs

### Merged CheckList.Api into CheckList.Web (Single-App Architecture)
- Consolidated the separate API project into the Blazor Server app — now one deployable unit
- Moved Data (DbContext, Entities, Repositories), Controllers, Hubs, and DTOs from Api to Web
- Replaced HTTP-based `CheckListApiClient` with direct `CheckListService` that calls repositories in-process
- SignalR hub now hosted by the Web app itself — CheckListView connects to local `/hubs/checklist`
- Controllers still serve the same REST API surface for any external callers
- Updated Aspire AppHost to only wire CheckList.Web (no more CheckList.Api reference)
- Import/Export DTOs now use proper JsonPropertyName attributes from the Api canonical versions
- All namespaces updated from `CheckList.Api.*` to `CheckList.Web.*`
- `ICheckListApiClient` interface kept for Blazor page compatibility, implementation swapped to direct repo calls

### 2026-04-22 - Editable Checklist Name on Template Activation
- ListSelectionDialog.razor now includes a text input for the checklist name, defaulting to `"{DateTime.Now:ddd MMM d} {templateName}"`
- Changed `OnConfirm` callback from `EventCallback<List<int>>` to `EventCallback<(string Name, List<int> ListIds)>` (value tuple)
- `customName` parameter added through the full stack: ICheckListApiClient → CheckListService → ICheckRepository → CheckRepository → Controller
- `ActivateCheckSetRequest` record extended with optional `CustomName` field (backward compatible)
- Repository uses customName when non-empty, otherwise falls back to existing auto-generated format
- CSS for name input uses only CSS variables (--text-primary, --bg-card, --border-color, --accent-color) — no hardcoded colors
- 3 new tests added (147 total, up from 144): custom name used, null fallback, whitespace fallback

### 2026-04-22 - RV Outdoors Theme & About Page (Poncho)
Frontend team completed theme refresh and About page:
- Entire Bootstrap palette replaced with warm RV/campground colors: forest green primary, sky blue secondary, earth brown accents
- All colors now via CSS variables in app.css — supports dark/light themes automatically
- New `/about` page with hero, feature cards, tech stack, and CTA sections
- "About" link added to sidebar and mobile bottom nav
- Card color convention applied: sky-blue = info, earth-brown = technical, forest-green = CTA
