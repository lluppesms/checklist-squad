# Billy — History

## Project Context
- **Owner:** Lyle Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, Azure DevOps + GitHub Actions
- **Repo:** lluppesms/checklist-squad

## Learnings

- RigRoll should be presented as a real-time shared RV camping checklist app whose user-facing story centers on reducing setup and teardown coordination chaos between two people.
- Key architectural talking points for presentations: single-app Blazor Server + merged ASP.NET Core API, in-process SignalR hub, EF Core against Azure SQL using the `[CheckList]` schema, and Microsoft Entra ID via Microsoft.Identity.Web.
- Infrastructure and delivery narrative should highlight `infra/Bicep/` AVM-based modules, Azure App Service + Azure SQL hosting, and dual CI/CD implementations in `.github/workflows/` and `.azdo/pipelines/`.
- Lyle prefers polished standalone artifacts in-repo; the presentation lives at `docs/presentation/index.html` and uses reveal.js CDN, Mermaid CDN, speaker notes, and print-friendly styling.
- Team storytelling should reference the Squad roles from `.squad/team.md`, especially Dutch, Poncho, Mac, Hawkins, Blain, and Billy as specialized AI collaborators.
