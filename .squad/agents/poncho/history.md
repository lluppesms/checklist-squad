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

### 2026-04-21: Selective List Activation UI
Added list selection dialog when activating templates so users can choose which lists to activate instead of always activating all lists:
- **Model Update**: Changed `ActivateCheckSetRequest` to include optional `List<int>? SelectedListIds` parameter (backward compatible — null = activate all)
- **API Client**: Updated `ActivateCheckSetAsync` signature to pass selected list IDs to API
- **ListSelectionDialog Component**: New modal component at `/Components/Shared/ListSelectionDialog.razor` with:
  - All lists pre-checked by default for quick activation
  - Individual checkboxes with list name + description
  - "Select All" / "Deselect All" toggle
  - Selection counter ("3 of 7 lists selected")
  - "Activate Selected" button (disabled if nothing selected)
  - Mobile-friendly full-width layout on small screens
  - CSS variables for dark/light theme support
- **Templates.razor Flow**: Changed from one-click activation to two-step:
  1. Click "Activate" → fetches template details → shows ListSelectionDialog
  2. User selects lists → clicks "Activate Selected" → activates only those lists
- **UX Pattern**: User-focused selection with sensible defaults (all checked) that still allows surgical control for days when only 1-2 lists are needed

**Design Decisions**:
- Checkboxes sized 1.25rem for easy mobile tapping
- Modal max-height 90vh with scrollable list section
- Full-screen on mobile (no awkward small modal on phone)
- Disabled activation button when no lists selected (prevents empty activation)
- Clear visual feedback on list hover and selection state

### 2026-07-27: RV Outdoors Theme Refresh
Replaced the bland default Bootstrap blue theme with a warm, nature-inspired RV/campground palette:
- **Palette source**: `wwwroot/images/RV-Checklist.jpg` — cartoon RV at lakeside campground
- **Primary accent**: Forest green `#4A8C5C` (was `#0d6efd`), dark: `#6dba80`
- **Secondary accent**: Sky blue `#6CB4D9`, earthy brown `#8B6F47`
- **Navbar/sidebar**: Dark charcoal `#3A3A3A` with cream `#F5F0E8` text
- **Card pattern**: Colored left borders (green=checklists, blue=active, brown=templates), subtle gradient backgrounds, hover lift
- **Progress bars**: Forest green, not blue
- **Typography**: Georgia serif for headings, Segoe UI for body
- **Extended CSS variables**: `--sky-blue`, `--forest-green`, `--earth-brown`, `--cream`, `--charcoal`, `--navbar-bg`, `--sidebar-bg`, `--sidebar-active-bg`, `--card-accent-border`, `--card-gradient-start/end`, plus `-hover` and `-subtle` variants for each color
- **Home page hero**: `RV-Checklist.jpg` in rounded container with shadow, camping-themed welcome messages
- **Empty states**: Dashed borders, outdoor icons (compass, signpost), friendly RV copy
- **Dark theme**: Full parallel palette with appropriate brightness shifts
- **Rule**: All colors via CSS vars only — zero hardcoded hex in component CSS files

### 2026-07-27: About Page
Created `/about` page with camping-themed content:
- **Files**: `About.razor` + `About.razor.css` in `Components/Pages/`
- **Sections**: Hero (reuses RV-Checklist.jpg), "What Is This?", 4-card feature grid, tech stack, closing CTA
- **Card color coding**: Sky-blue border for "What Is This?", earth-brown for tech stack, forest-green for CTA — matches existing card-border convention
- **Feature grid**: 1-col on mobile, 2-col at 576px+ breakpoint
- **Navigation**: Added "About" with `bi-info-circle` to both NavMenu (desktop sidebar) and MainLayout bottom-nav (mobile)
- **Zero hardcoded colors**: All CSS uses vars from app.css
- **Pattern**: Mirrors Home page hero structure (image-wrapper, title, description) for visual consistency

