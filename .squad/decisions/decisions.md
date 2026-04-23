# Decisions — Checklist Squad

---

## Partnership-Based Sharing Backend Implementation

**Author:** Mac (Backend Dev)  
**Date:** 2026-04-22  
**Status:** Implemented

### Decision

Implemented a complete partnership-based sharing system where invites create bidirectional relationships that auto-share all checklists (both existing and future) between partners.

### Architecture

#### Data Model
- **SharingInvite**: Stores invite token hash (SHA256), sender, recipient email, role, status (pending/accepted/expired/revoked), 7-day expiry
- **UserPartnership**: Directional relationship (TWO rows per partnership: A→B and B→A), stores role, auto-share flag, invite provenance
- **CheckSetShare**: Extended with optional PartnershipId for provenance tracking (enables cascade cleanup on revoke)

### Key Design Decisions

1. **Bidirectional Partnerships**: Each accepted invite creates TWO UserPartnership rows (sender grants to acceptor + acceptor grants to sender) allowing symmetric access and future role differentiation

2. **Auto-Share Strategy**:
   - On invite accept: Auto-share ALL existing active checklists for both users
   - On new checklist create: Auto-share to all active partners
   - Provenance tracked via PartnershipId for clean cascade delete

3. **Token Security**:
   - 32-byte random tokens (Base64Url encoded)
   - Only SHA256 hash stored in database (raw token never persisted)
   - 7-day expiry with validation on acceptance

4. **Revocation**:
   - Deletes BOTH directional UserPartnership rows
   - Cascades to delete ONLY auto-created CheckSetShare rows (via PartnershipId)
   - Preserves manually created shares

5. **Email Integration**:
   - IEmailService interface defined
   - NoOpEmailService logs invite links (ready for SMTP/SendGrid swap)

### Files Changed

**New Entities** (3):
- `Data/Entities/SharingInvite.cs`
- `Data/Entities/UserPartnership.cs`
- Modified `Data/Entities/CheckSetShare.cs` (added PartnershipId)

**Repository Layer** (2):
- `Data/Repositories/ISharingRepository.cs`
- `Data/Repositories/SharingRepository.cs`

**Service Layer** (4):
- `Services/ISharingService.cs` (includes DTOs: SharingInviteResult, PartnerDto)
- `Services/SharingService.cs`
- `Services/IEmailService.cs`
- `Services/NoOpEmailService.cs`

**API Controller** (1):
- `Controllers/SharingController.cs` (4 endpoints)

**Schema Migration**:
- `Data/DatabaseSchemaService.cs` (added 3 new tables + PartnershipId column)

**Auto-Share Hook**:
- Modified `Services/CheckListApiClient.cs` to call AutoShareNewCheckSetAsync after activation

**DI Registration**:
- `Program.cs` (registered ISharingRepository, ISharingService, IEmailService)

### API Surface

```
POST /api/sharing/invite
  Body: { recipientEmail, role? }
  Returns: { inviteToken, inviteLink, expiresAt }

POST /api/sharing/accept/{inviteToken}
  Returns: { success, partnerName?, error? }

GET /api/sharing/partners
  Returns: List<PartnerDto>

DELETE /api/sharing/partners/{partnerUserId}
  Returns: 204 No Content
```

### Impact

- Frontend team (Poncho) can now build invite/accept UI
- Partners automatically see each other's new checklists
- Revoke partnership cleanly removes auto-shares but preserves manual shares
- Ready for email service integration (swap NoOpEmailService for real SMTP)

---

## Sharing UI Implementation

**Author:** Poncho (Frontend Dev)  
**Date:** 2026-07-28  
**Status:** Implemented

### Decision

Built complete sharing UI for checklist collaboration, enabling users to invite partners, accept invitations, view sharing partners, and see sharing status throughout the app.

### Components Created

#### 1. InvitePartnerDialog.razor + .razor.css
Modal dialog for creating sharing invites:
- Email input with client-side validation
- Role selection via radio buttons
- Two-stage flow: Input email + select role → Display invite link
- Copy to clipboard support
- Expiry info display

#### 2. AcceptInvite.razor + .razor.css
Public page at `/invite/{token}` handling invite acceptance:
- Unauthenticated flow: Welcome message → "Sign In to Accept"
- Authenticated flow: Auto-processes invite on load
- Success/error messaging

#### 3. PartnersPanel.razor + .razor.css
Panel showing current sharing partners:
- Partner cards with avatar, name, email, role badge, partnership date
- Remove button with ConfirmDialog
- Empty state messaging

#### 4. ActiveCheckSets.razor Integration
- "Invite Partner" button in header
- Collapsible "Sharing Partners" section
- "Shared" badge on non-owned checklists

#### 5. CheckListView.razor Integration
- Sharing info banner when viewing shared checklist
- Shows owner name and user's role

### DTO Changes

Extended `CheckSetSummaryDto` and `CheckSetDto` to include `OwnerId` for determining shared checklists.

### Design Decisions

- **Role Labeling**: Internal: "user"/"admin", User-facing: "Crew Member"/"Co-Captain"
- **Color Scheme**: Sky blue for sharing-related UI (informational), forest-green for Co-Captain badge
- **Mobile Optimization**: Full-screen dialogs, 44px touch targets, vertical stacking
- **Security**: Public AcceptInvite page for unauthenticated welcome, token visible but single-use

### Files Changed

**Created:**
- `Components/Shared/InvitePartnerDialog.razor` + `.razor.css`
- `Components/Shared/PartnersPanel.razor` + `.razor.css`
- `Components/Pages/AcceptInvite.razor` + `.razor.css`

**Modified:**
- `Components/Pages/ActiveCheckSets.razor` + `.razor.css`
- `Components/Pages/CheckListView.razor` + `.razor.css`
- `Models/CheckSetDto.cs`
- `Models/Mapping/DtoMapper.cs`

### Impact

- Users can invite partners and build sharing relationships
- Shared checklists automatically visible with clear ownership attribution
- Role-based UI adapts to user permissions

---

## Sharing Feature Test Coverage

**Author:** Hawkins (Tester)  
**Date:** 2026-04-22  
**Status:** Implemented

### Summary

Comprehensive unit test coverage added for the sharing feature across repository, service, and controller layers. 65 new tests created following established MSTest patterns.

### Test Files Created

#### 1. SharingRepositoryTests.cs (17 tests)
Tests EF Core repository layer using in-memory database:
- Invite management: Create, retrieve, list, accept
- Partnership management: Create, retrieve, revoke
- Share management: Create with PartnershipId, retrieve, delete by partnership

#### 2. SharingServiceTests.cs (31 tests)
Tests business logic using Moq mocks:
- Invite creation: Token generation, SHA256 hashing, 7-day expiry
- Invite acceptance: Happy path, rejection scenarios
- Partner management: DTO mapping, revocation
- Auto-sharing: New checkset sharing to all partners
- Access control: Owner access, shared user access, role enforcement

#### 3. SharingControllerTests.cs (17 tests)
Tests API endpoints using Moq mocks:
- POST /api/sharing/invite: Returns invite link, sends email
- POST /api/sharing/accept/{token}: Processes acceptance
- GET /api/sharing/partners: Returns partner list
- DELETE /api/sharing/partners/{id}: Removes partnership

### Test Infrastructure

**DbContextHelper Enhancement**: Added transaction warning suppression for in-memory testing of transactional code.

**Coverage:** 144 baseline tests → 209 total tests (+65)

### Edge Cases Covered

- Expired invites rejected
- Already-used invites rejected
- Email mismatch rejection
- Invalid tokens rejected
- New acceptor gets AppUser created
- Bidirectional partnerships created
- Auto-sharing to all partners with AutoShareEnabled=true
- Role-based access control enforcement
- Partnership revocation cleans up auto-shares

### Bugs Found & Fixed

**InvitePartnerDialog.razor Syntax Error**: Lambda expressions used double quotes inside double-quoted attributes. Fixed by changing outer quotes to single quotes.

### Impact

- All 209 tests passing
- No regression in existing tests
- Sharing feature fully covered at all layers
- Ready for integration testing

---

## UI-Only Rename — "Templates" → "Blueprints"

**Author:** Poncho (Frontend Dev)  
**Date:** 2026-07-28  
**Status:** Implemented

### Decision

Changed all user-facing text from "Templates" to "Blueprints" without touching backend.

### What Changed

- Page titles, headings, nav labels, button labels
- Dialog titles, error/success messages
- Export filenames

### What Stayed the Same

- URL routes (`/templates`) — avoid breaking bookmarks
- CSS class names — no user impact
- C# names, API endpoints, database schema

### Impact

- UI now consistently uses "Blueprints" terminology
- Backend unchanged, no code dependencies
- If renamed again, update same set of .razor files + Playwright tests

---

## localStorage Nickname Persistence

**Author:** Poncho (Frontend Dev)  
**Date:** 2026-07-27  
**Status:** Implemented

### Decision

User nicknames now persisted in browser localStorage under `checklist-nickname`. On circuit restart, `NicknamePrompt` checks localStorage via JS interop and skips overlay if nickname exists.

### Files Changed

- `src/CheckList.Web/wwwroot/js/BlazorInterop.js` — added nickname storage helpers
- `src/CheckList.Web/Components/Shared/NicknamePrompt.razor` — localStorage integration

### Trade-offs

- Per-browser/per-device storage (users re-enter nickname on new device)
- `checkedStorage` flag prevents overlay flash during JS interop

### Impact

- No API changes
- Improved UX: seamless nickname restoration across page refreshes

---

## App Rebranded from "Shared Checklist" to "RigRoll"

**Author:** Poncho (Frontend Dev)  
**Date:** 2026-07-28  
**Status:** Implemented

Rebranded entire app:
- Navbar brand, `<PageTitle>` components
- Home hero: headline + italic sub-headline + body paragraph
- About page updated with RigRoll messaging
- Playwright tests updated to assert on new brand name

### Impact

- New pages must use `<PageTitle>Page Name — RigRoll</PageTitle>` pattern
- MainLayout.razor brand is single source of truth
- Playwright tests verify brand presence

---

## Playwright Smoke Test Suite

**Author:** Hawkins (Tester)  
**Date:** 2026-04-22  
**Status:** Implemented

### Decision

Created separate MSTest project with 35 Playwright smoke tests targeting live dev deployment, complementing 209 unit tests.

### Key Choices

1. **Microsoft.Playwright** (not MSTest-specific package) — avoids MSTest 2.2.7 conflicts
2. **Sequential execution** (Workers=1) — stable under dev server constraints (~7 minutes)
3. **Per-test browser isolation** — eliminates cross-test state leakage
4. **Blazor circuit detection** — waits for SignalR connection before interaction
5. **Retry-based input** — handles Blazor binding timing gaps
6. **Resilient assertions** — accept populated OR empty-state data

### Consequences

- CI/CD can run post-deploy smoke tests
- Tests independent of database seed state
- BASE_URL configurable via env var
- 7-minute runtime acceptable for post-deploy, too slow for PR gates

---

## ADR-001: Template CRUD API Architecture

**Date:** 2026-04-21  
**Agent:** Mac (Backend Dev)  
**Status:** Implemented

### Context
App previously had read-only template support. Need full CRUD for 4-level template hierarchy plus import/export capabilities.

### Decision
Implemented RESTful API with hierarchical routing:
- Template Set: `POST/PUT/DELETE /api/templates/{setId}`
- Template List: `POST/PUT/DELETE /api/templates/{setId}/lists` and `/api/templates/lists/{listId}`
- Template Category: `POST/PUT/DELETE /api/templates/lists/{listId}/categories` and `/api/templates/categories/{categoryId}`
- Template Action: `POST/PUT/DELETE /api/templates/categories/{categoryId}/actions` and `/api/templates/actions/{actionId}`

Import/Export endpoints separated into dedicated controllers:
- `/api/templates/export` - Export all templates
- `/api/templates/{setId}/export` - Export single template set
- `/api/templates/import` - Import single template set
- `/api/export/full` - Export templates + active checklists
- `/api/import/full` - Import full backup

### Rationale
- Hierarchical routing makes parent-child relationships explicit
- Separate DTOs for requests vs responses prevents over-posting vulnerabilities
- Export DTOs use System.Text.Json attributes for clean JSON serialization
- All write operations set audit fields (CreateDateTime, CreateUserName, ChangeDateTime, ChangeUserName) consistently
- Repository returns DTOs directly to keep controllers thin
- Cascading deletes in EF Core configuration handle cleanup automatically

### Alternatives Considered
- Flat routing (`/api/templatesets`, `/api/templatelists`) - rejected as less intuitive for hierarchical data
- Single import/export endpoint - rejected to maintain separation of concerns between template management and full backup/restore

### Impact
- Frontend can now manage templates directly without database access
- Full backup/restore capability for disaster recovery
- Ready for authentication integration (currently uses "System" as default user)

---

## ADR-002: Template Editor UX Pattern

**Date:** 2026-04-21  
**Agent:** Poncho (Frontend Dev)  
**Status:** Implemented

### Context
Mac built the full template CRUD backend API with 4-level hierarchy (Set→List→Category→Action). Users need to create and edit complex checklist templates with potentially dozens of actions across multiple categories and lists.

### Decision
Implemented a **single-page hierarchical editor** rather than separate pages for each level.

**Key UX choices:**
1. **Inline editing**: All fields editable directly in place — no modal dialogs
2. **Visual hierarchy**: Lists as cards, categories within, actions nested further
3. **Positional controls**: Up/down arrows for reordering instead of drag-and-drop (better for touch devices)
4. **Progressive disclosure**: Categories collapse within lists, actions within categories
5. **Mobile-first**: Responsive stacking on small screens, full layout on desktop
6. **Theme agnostic**: All colors via CSS variables (`var(--bg-card)`, `var(--text-primary)`) — never hardcoded hex/rgb

### Rationale
- **Speed**: Users can see the full structure and edit multiple levels without page transitions
- **Touch-friendly**: Arrow buttons work better than drag handles on phones
- **Accessibility**: Keyboard navigation works naturally with buttons
- **Consistency**: Matches the pattern from dadabase.demo (form-based editing with action buttons)

### Trade-offs
- **Complexity**: Single-page editor is more complex than wizard-style flow
- **Performance**: Could be slow with 100+ items, but typical use is <50 actions per template
- **Learning curve**: New users might be overwhelmed initially, but power users will be faster

### Implementation Notes
- Save logic does full diff: creates new items (Id=0), updates existing, deletes missing
- Import/Export uses timestamped filenames: `Templates_Export_20260421_160000.json`
- File downloads via JS interop (`downloadFileFromBase64`) for browser compatibility
- 10MB file size limit for imports (configurable in `InputFile.OpenReadStream`)

### Future Considerations
- If templates grow to 100+ actions, consider virtualization or pagination
- Could add bulk operations (e.g., "duplicate list") if users request it
- Drag-and-drop reordering is possible but adds complexity for touch devices

---

## ADR-003: Project Architecture & Structure

**Date:** 2025-07-18 (Plan), Confirmed 2026-04-21  
**Agent:** Dutch (Lead)  
**Status:** ACTIVE / Reference

### Context
Building shared checklist app for real-world use (campsite checklist coordination). Need to align structure with proven patterns from dadabase.demo while accommodating new technologies (Aspire, SignalR).

### Decision
Follow dadabase.demo patterns, adapted for our domain:

```
checklist-squad/
├── .azuredevops/pipelines/         # Azure DevOps YAML pipelines + templates
├── .github/
│   ├── workflows/                  # Numbered entrypoints + template reusables
│   ├── copilot-instructions.md
│   └── config/                     # Workflow variable files
├── Docs/                           # Documentation and images
├── infra/
│   └── Bicep/
│       ├── main.bicep
│       ├── main.bicepparam
│       ├── resourcenames.bicep
│       └── modules/
│           ├── database/           # Azure SQL
│           ├── webapp/             # App Service (or Container App)
│           ├── monitor/            # App Insights + Log Analytics
│           ├── security/           # Key Vault
│           └── signalr/            # Azure SignalR Service
├── src/
│   ├── CheckList.AppHost/          # Aspire orchestrator project
│   ├── CheckList.ServiceDefaults/  # Aspire shared service defaults
│   ├── CheckList.Api/              # ASP.NET Core Web API + SignalR hub
│   ├── CheckList.Web/              # Blazor frontend (Server or WASM)
│   ├── CheckList.Database/         # SQL DACPAC (.sqlproj)
│   └── CheckList.Tests/            # MSTest unit/integration tests
├── old-source/                     # Preserved prototype reference
├── README.md
└── checklist-squad.sln
```

### Key Patterns
- **SQL DACPAC** uses `Microsoft.Build.Sql` SDK v2.0.0
- **EF Core** models align with DACPAC schema (DACPAC is source of truth)
- **Folder convention:** `dbo/Tables/`, `dbo/Stored Procedures/`, `dbo/Views/`
- **Repository pattern** for data access
- **SignalR hub** for real-time updates
- **System.Text.Json** (no Newtonsoft.Json)

### Namespace Conventions

| Project | Root Namespace |
|---|---|
| CheckList.Api | `CheckList.Api` |
| CheckList.Web | `CheckList.Web` |
| CheckList.Database | N/A (SQL project) |
| CheckList.Tests | `CheckList.Tests` |
| CheckList.AppHost | `CheckList.AppHost` |
| CheckList.ServiceDefaults | `CheckList.ServiceDefaults` |

Sub-namespaces follow folder structure:
- `CheckList.Api.Hubs` — SignalR hub
- `CheckList.Api.Models` — Entity models and DTOs
- `CheckList.Api.Repositories` — Data access interfaces + implementations
- `CheckList.Api.Services` — Business logic layer
- `CheckList.Web.Components` — Blazor components
- `CheckList.Web.Pages` — Blazor pages

### Key Technology Choices

| Component | Choice | Version Target |
|---|---|---|
| .NET | .NET 9 (or latest LTS) | Latest stable |
| Blazor | Server-side interactive | Built-in |
| EF Core | 9.x | Match .NET version |
| SignalR | ASP.NET Core SignalR | Built-in |
| SQL DACPAC | Microsoft.Build.Sql 2.0.0 | Match dadabase.demo |
| Aspire | Latest stable | .NET Aspire 9.x |
| Bicep | Latest | Azure CLI integrated |
| Testing | MSTest 3.x | Per user preference |

### Impact
- Consistent with industry patterns (matches dadabase.demo)
- Scalable to Azure deployment
- Team can leverage existing knowledge
- Clear separation of concerns
- Testable architecture (repository pattern, DI)

---

## Open Questions

1. **Authentication:** Is this app open (no login) or does it need user auth? The prototype has auth infrastructure but the use case (two people at a campsite) suggests it could be simpler.
2. **Hosting model:** App Service vs Container App? dadabase.demo supports both. App Service is simpler for this use case.
3. **Azure SignalR Service:** Use managed service or self-hosted? Managed is more reliable for mobile connections with poor connectivity.
4. **.NET version:** Pin to .NET 9 LTS or use .NET 10 preview? Recommend .NET 9 for stability.

---

*Updated: 2026-04-21 — Merged Phase 3-4 decisions from Mac (backend) and Poncho (frontend).*
