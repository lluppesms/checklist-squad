# Azure DevOps Pipelines

This folder contains the Azure DevOps pipeline definitions for the Checklist Squad project.

## Pipelines

| File | Description |
| --- | --- |
| `1-deploy-bicep.yml` | Deploy Bicep infrastructure to a target environment |
| `2-build-deploy-webapp.yml` | Build and deploy the API + Web application |
| `4-build-deploy-dacpac.yml` | Build and deploy a SQL DACPAC to Azure SQL |
| `6-pr-scan-build.yml` | PR validation: build, test, and Bicep scan |

## Subfolders

| Folder | Description |
| --- | --- |
| `steps/` | Reusable step templates shared across pipelines |
| `vars/` | Variable templates for shared configuration |

## Variable Groups

These pipelines reference the following Azure DevOps variable groups:

- **checklist-common** – Shared variables across all environments (e.g., service connections)
- **checklist-DEV / checklist-QA / checklist-PROD** – Environment-specific variable overrides

See the variable group definitions in your Azure DevOps project Library for the full list of values.
