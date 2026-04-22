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
- **Playwright smoke tests:** `src/CheckList.PlaywrightTests/` — separate project using Microsoft.Playwright 1.59.0 + MSTest 4.0.2. 35 tests across 7 test files targeting the live dev site. Chromium headless, sequential execution (Workers=1) to avoid overwhelming the dev server.
- **Blazor Server + Playwright gotchas:** (1) `FillAsync` doesn't trigger Blazor `@bind:event="oninput"` — use `PressSequentiallyAsync` instead. (2) Blazor Server prerender loads HTML instantly but SignalR circuit takes seconds — must wait for circuit before interacting. (3) Nickname entry needs retry loop because circuit may not be ready when overlay appears. (4) Session-scoped state (nickname) doesn't persist across browser contexts.
- **Playwright test base URL:** Configurable via `APP_URL` env var, defaults to `https://lsq-checklist1-dev.azurewebsites.net/`.
- **Playwright test structure:** `SmokeTests/SmokeTestBase.cs` manages browser lifecycle per-test and provides `NavigateAndWaitForBlazor()` with Blazor circuit detection + nickname dismissal.

## Recent Updates

📌 **2026-04-22:** Playwright smoke test suite created by Hawkins — 35 tests, all green against live dev site. Covers home page, navigation, checklists, templates, import/export, responsive layout, and SignalR connectivity.

📌 Team initialized on 2026-04-21
