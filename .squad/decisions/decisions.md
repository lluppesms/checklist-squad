# Decisions — Checklist Squad

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
