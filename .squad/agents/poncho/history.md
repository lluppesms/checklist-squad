# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-07-28: Sharing UI Complete
Built the complete sharing UI for the checklist collaboration feature:
- **InvitePartnerDialog.razor**: Modal dialog for creating sharing invites with email input, role selection (Crew Member/Co-Captain), and invite link generation with copy-to-clipboard functionality
- **AcceptInvite.razor**: Public page at `/invite/{token}` that handles unauthenticated welcome flow (redirects to login with returnUrl) and authenticated acceptance flow (auto-processes invite on load)
- **PartnersPanel.razor**: Collapsible panel showing current sharing partners with role badges, partnership dates, and remove functionality with confirmation
- **ActiveCheckSets.razor integration**: Added "Invite Partner" button in page header, collapsible "Sharing Partners" section above checklists grid, and "Shared" badge on checklists not owned by current user
- **CheckListView.razor integration**: Added sharing indicator showing "Shared with you by {owner}" and user's role (Crew Member/Co-Captain) when viewing shared checklists
- **DTO updates**: Extended CheckSetSummaryDto and CheckSetDto to include OwnerId for determining shared status
- **User-friendly role labels**: Internal "user"/"admin" mapped to "Crew Member"/"Co-Captain" throughout UI
- **Mobile-first design**: Full-screen dialogs on mobile, adequate tap targets (min 44px), responsive partner cards
- **Sky-blue accent**: Used `--sky-blue` for sharing-related UI elements (badges, borders, icons) per team convention for informational content
- **Clipboard JSInterop**: Uses `navigator.clipboard.writeText()` directly from Blazor component — no custom JS wrapper needed
- **AcceptInvite security**: Does NOT have `[Authorize]` attribute so unauthenticated users can see welcome message before signing in

**Key patterns applied**:
- Follow existing dialog patterns (ConfirmDialog, ListSelectionDialog) for overlay, close button, and footer actions
- CSS variables throughout — zero hardcoded colors
- Scoped `.razor.css` files for every component
- Direct service injection (`ISharingService`, `IUserIdentity`) — no HTTP calls
- Error handling with user-friendly messages
- Consistent border-left accent pattern (sky-blue for sharing)

### 2026-07-28: Blueprints Rename (UI-Only)
Renamed all user-facing references from "Templates" to "Blueprints" across the frontend per Lyle's request. No backend, database, or code-behind changes — purely display text.
- **Templates.razor**: Page title, h2 heading, button labels ("Create New Blueprint", "Create Your First Blueprint"), loading/error/empty messages, delete dialog title, export filename prefix
- **TemplateEditor.razor**: Page title, h2 heading, form label ("Blueprint Name"), placeholder, all error messages
- **NavMenu.razor**: Sidebar nav label → "Blueprints"
- **MainLayout.razor**: Bottom nav label → "Blueprints"
- **Home.razor**: Quick-link card description ("blueprint" instead of "template")
- **About.razor**: Feature card heading → "Blueprints", description text, "Activate a blueprint" copy
- **ActiveCheckSets.razor**: Empty-state link text → "Blueprints"
- **ImportExport.razor**: Export/import button labels, descriptions, success/error messages, export filename prefix
- **Playwright tests**: Updated 6 text assertions across TemplateTests.cs, NavigationTests.cs, ResponsiveTests.cs, ImportExportTests.cs to match new display text
- **Not changed**: URL routes (`/templates`), CSS classes (`.template-card`), C# identifiers, API endpoints, database schema
Rebranded the entire app from "Shared Checklist" to "RigRoll":
- **Files changed**: Home.razor, About.razor, MainLayout.razor, Templates.razor, TemplateEditor.razor, ImportExport.razor, Home.razor.css, plus 4 Playwright test files
- **Brand name**: "RigRoll" everywhere — navbar, PageTitle components, hero copy, about page
- **Home hero**: New three-part copy structure — headline ("Stop Shouting. Start RigRolling."), italic sub-headline, and body paragraph. Added `.hero-subheadline` CSS class for the italic callout.
- **About page**: Updated "What Is This?" → "What Is RigRoll?", "Shared Checklists" → "Crew Checklists", "Happy Camping!" → "Happy RigRolling!"
- **Playwright tests**: Updated all brand-name assertions (ConnectivityTests, HomePageTests, ImportExportTests, SmokeTestBase)
- **No color changes**: Kept existing RV outdoors palette (forest green, sky blue, earth brown) — all CSS vars, zero hardcoded values
- **Pattern**: Sub-headline uses `font-style: italic` with `var(--text-secondary)` — scoped in Home.razor.css

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

### 2026-07-27: localStorage Nickname Persistence
Added browser localStorage persistence for user nicknames so they survive page refreshes and revisits:
- **JS interop**: Added `nicknameStorage.get/set/clear` functions to `wwwroot/js/BlazorInterop.js`
- **NicknamePrompt.razor**: Now checks localStorage via `OnAfterRenderAsync(firstRender)` before showing the overlay
- **Flow**: On first render → check localStorage → if nickname found, set `UserIdentityService` and skip overlay → if not found, show overlay as before → on manual entry, save to both service and localStorage
- **Key detail**: `checkedStorage` flag prevents a flash of the overlay before JS interop completes (overlay hidden until localStorage check finishes)
- **localStorage key**: `checklist-nickname`
- **No breaking changes**: Users without a stored nickname still see the overlay exactly as before

### 2026-04-22: Editable Checklist Name on Template Activation
Added editable name field to ListSelectionDialog:
- Text input defaults to `"{DateTime.Now:ddd MMM d} {templateName}"`
- `OnConfirm` callback now returns `(string Name, List<int> ListIds)` tuple instead of just `List<int>`
- Full stack wired: dialog → service → controller → repository
- `ActivateCheckSetRequest` extended with optional `CustomName` field (backward compatible)
- Repository falls back to auto-generated name when customName is null/whitespace
- CSS for name input uses only CSS variables — no hardcoded colors
- 3 new tests added for name customization scenarios (147 total now passing)

