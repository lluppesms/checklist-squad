# 🏕️ Checklist Squad

**A real-time shared checklist app — born from the chaos of RV camping.**

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](#) [![.NET](https://img.shields.io/badge/.NET-10.0-purple)](#) [![Blazor](https://img.shields.io/badge/Blazor-Server-blue)](#) [![License](https://img.shields.io/badge/license-MIT-green)](#)

---

## 🤔 Why Does This Exist?

Have you ever tried to set up an RV at a campground? There are approximately *four thousand* things to do, and they all need to happen in a specific order. Forget to unhook the sewer line before pulling out? Congratulations, you've just made enemies for life. And possibly ruined your shoes.

Meanwhile, there are *two people* trying to coordinate this dance of checklists while one of them is crawling under the vehicle and the other is yelling "DID YOU TURN OFF THE WATER PUMP?!" from across the campsite.

**Checklist Squad** was created so couples (or anyone brave enough to own an RV) can share a real-time checklist on their phones. When one person checks off "Level the RV," the other person sees it instantly — no yelling across the campground required.

> *Pro tip: While waiting for your partner to finish the checklist, grab a lawn chair and watch someone three sites down attempt to back their 40-foot fifth wheel into a spot designed for a tent. It's free entertainment. Bring popcorn. 🍿*

---

## 🎯 What It Does

- **Templated Checklists** — Pre-built lists for arrival, departure, interior, exterior, and vehicle tasks
- **One-Tap Activation** — Pick a template, get a fresh copy for this trip — no recreating lists from memory
- **Real-Time Sync** — Powered by SignalR, so when your partner checks off "Extend the awning," you see it immediately on your phone (even if you're busy pretending to supervise)
- **Mobile-First Design** — Because nobody is setting up an RV with a laptop open on the picnic table
- **Categories & Ordering** — Tasks are grouped and ordered so you don't accidentally try to retract the slides before disconnecting the power

> *Nothing brings a couple closer together than one person insisting "I ALREADY checked that off" while the other person's phone clearly shows it unchecked. SignalR doesn't lie, Karen.*

---

## 🏗️ Tech Stack

| Layer | Technology |
| --- | --- |
| **Frontend** | Blazor Server (.NET 10) |
| **Backend API** | ASP.NET Core Web API |
| **Real-Time** | SignalR |
| **Database** | Azure SQL Database |
| **Orchestration** | .NET Aspire |
| **Infrastructure** | Bicep (Azure IaC) |
| **CI/CD** | GitHub Actions + Azure DevOps Pipelines |
| **Hosting** | Azure App Service |

---

## 📁 Project Structure

```
checklist-squad/
├── src/
│   ├── CheckList.Web/          # Blazor Server frontend
│   ├── CheckList.Api/          # ASP.NET Core Web API
│   ├── CheckList.Database/     # SQL DACPAC project
│   ├── CheckList.AppHost/      # .NET Aspire orchestration
│   ├── CheckList.ServiceDefaults/ # Shared service config
│   └── CheckList.Tests/        # Unit tests
├── infra/
│   └── Bicep/                  # Azure infrastructure as code
├── .github/
│   ├── workflows/              # GitHub Actions CI/CD
│   ├── CreateGitHubSecrets.md  # Required secrets & variables
│   └── workflows-readme.md     # Workflow setup guide
├── .azdo/
│   └── pipelines/              # Azure DevOps CI/CD
└── specifications.md           # Original app requirements
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)
- A SQL Server instance (local or Azure)

### Run Locally

```bash
# Clone the repo
git clone https://github.com/lluppesms/checklist-squad.git
cd checklist-squad

# Run with Aspire
dotnet run --project src/CheckList.AppHost
```

The Aspire dashboard will open and show you the running services. The web app will be available at the URL shown in the dashboard.

> *Running locally is significantly less stressful than backing an RV into a spot with your spouse giving hand signals that may or may not mean "turn right." Although, if the build fails, the hand signals you make at your monitor may be similar.*

---

## ☁️ Deployment

Infrastructure is deployed via Bicep and the CI/CD pipelines handle build + deploy.

### GitHub Actions

See [.github/workflows-readme.md](.github/workflows-readme.md) for workflow setup and [.github/CreateGitHubSecrets.md](.github/CreateGitHubSecrets.md) for required secrets and variables.

### Azure DevOps

See [.azdo/pipelines/readme.md](.azdo/pipelines/readme.md) for pipeline setup and required variable groups.

---

## 📋 How Checklists Work

1. **Templates** are predefined lists of tasks (e.g., "Arrival — Exterior", "Departure — Vehicle")
2. When you're ready, you **activate** a template — this creates a fresh instance for this trip
3. Both users open the active checklist on their phones
4. Check off tasks as you complete them — **updates appear instantly** on the other person's screen
5. When everything is checked off, you're good to go! 🚐💨

> *The app does NOT include a "Watch the neighbor try to parallel park their Class A motorhome" checklist, but honestly, that activity doesn't need a checklist. Just a chair, a beverage, and the patience of a saint. We've seen a guy do a 47-point turn. FORTY. SEVEN. At that point, it's not parking — it's performance art. 🎭*

---

## 🤝 Contributing

Contributions are welcome! Whether you're fixing bugs, adding features, or just correcting the spelling of "sewer hookup" in a checklist template.

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📜 Reference

This project uses the [lluppesms/dadabase.demo](https://github.com/lluppesms/dadabase.demo) repository as the golden code reference for pipeline structure, Bicep conventions, and CI/CD patterns.

See [specifications.md](specifications.md) for the original application requirements and design decisions.

---

## 📝 License

This project is licensed under the MIT License — because sharing is caring, just like sharing a checklist with your camping partner.

---

> *Remember: Friends don't let friends leave the campground with the TV antenna still up. That's what Checklist Squad is for.* 🏕️📡
