# Project Context

- **Owner:** Lab Luppes
- **Project:** Shared Checklist App — Real-time RV campground checklists with SignalR sync. Users on phones check off tasks in real-time as they set up or break down camp.
- **Stack:** C# Blazor, ASP.NET Core Web API, SignalR, Azure SQL, Aspire, Bicep, GitHub Actions, Azure DevOps pipelines
- **Reference Repo:** lluppesms/dadabase.demo (gold standard for project structure, pipelines, Bicep)
- **Old Prototype:** `old` branch has a working prototype — good DB schema, but code needs full rebuild
- **Created:** 2026-04-21

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- **Test project:** `src/CheckList.Tests/` uses MSTest 4.0.1, Moq 4.20.72, EF Core InMemory 10.0.6. MSTest 4.x uses `Assert.ThrowsExactlyAsync` (not `ThrowsExceptionAsync`).
- **Test organization:** One test file per class under Services/, Repositories/, Controllers/, Models/, Hubs/. Shared `DbContextHelper` in Helpers/ provides in-memory DbContext + seed data.
- **Coverage exclusions:** `coverage.runsettings` must exclude `**/Components/**` for Blazor UI code (not `Website/` which is from the dadabase reference project).
- **Repository pattern:** Controllers talk directly to repositories (ICheckRepository, ITemplateRepository) + IHubContext for SignalR. The service layer (CheckListService) wraps repos + hub for Blazor component use.
- **Key testing patterns:** Repositories tested with EF Core InMemory provider. Services/controllers tested with Moq. SignalR hub tested by mocking IHubCallerClients + IGroupManager.
- **Coverage baseline:** 144 tests, 94.5% line / 92% branch on testable code (excluding Blazor UI, Extensions.cs, Program.cs).

## Recent Updates

📌 **2026-04-22:** Unit test suite completed by Hawkins — 144 MSTest tests, 94.5% line / 92% branch coverage, 10 test files. All tests green, build clean.

📌 Team initialized on 2026-04-21
