# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-04-21: Template Editor & Import/Export UI Complete
Built the complete frontend for template management, including:
- **Request DTOs**: Created `TemplateRequestDtos.cs` and `ImportExportDtos.cs` mirroring Mac's API contracts
- **API Client**: Extended `ICheckListApiClient` and `CheckListApiClient` with full CRUD methods for all 4 levels (Set/List/Category/Action) plus import/export endpoints
- **Templates Page**: Updated with Create/Edit/Delete/Export buttons per template card
- **Template Editor**: Full hierarchical CRUD editor at `/templates/edit/{setId?}` with inline editing, drag-to-reorder (via up/down arrows), and nested management of Lists→Categories→Actions
- **Import/Export Page**: Dedicated UI at `/import-export` for exporting all templates, full exports (templates+checklists), and importing from JSON files with 10MB limit
- **JS Interop**: Created `BlazorInterop.js` with `downloadFileFromBase64` for client-side file downloads with timestamped filenames
- **Navigation**: Added Import/Export to NavMenu with appropriate icon

**Key Patterns Applied**:
- Mobile-first responsive design with collapsible sections
- CSS variables throughout — zero hardcoded colors (supports dark/light themes automatically)
- Bootstrap spacing utilities (m-*, p-*) for consistent padding
- Scoped CSS via `.razor.css` pattern for every component
- File-scoped namespaces, `var` over explicit types, `[Inject]` property pattern
- Reused existing `ConfirmDialog` and `LoadingSpinner` components for consistency
- Timestamped export filenames: `Templates_Export_20260421_160000.json` pattern

