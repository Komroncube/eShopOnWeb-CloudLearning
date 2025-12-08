# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Added
- **Infrastructure**: Added Azure Container Registry (ACR) support in `infra/core/host/containerregistry.bicep`.
- **Infrastructure**: Added ACR role assignment module in `infra/core/security/acr-access.bicep`.
- **CI/CD**: Created `.github/workflows/deploy-container.yml` for containerized deployment to Azure App Service.
  - Supports deploying both `web` and `api` projects via workflow dispatch inputs.
  - Uses Azure OIDC authentication.
  - Builds Docker images and pushes to ACR.
  - Deploys to Azure App Service using the container image.

### Changed
- **Infrastructure**: Updated `infra/main.bicep` to:
  - Include ACR module.
  - Configure App Service for Linux Containers.
  - Use Managed Identity for pulling images from ACR.
  - Use `B1` (Basic) App Service Plan (lowest-cost Linux container option).
  - Commented out SQL Server resources (Catalog and Identity databases) to use In-Memory database instead.
- **Application**: Updated `src/Web/Extensions/ServiceCollectionExtensions.cs` to prioritize `UseOnlyInMemoryDatabase` configuration, ensuring In-Memory DB is used even in production environments if configured.
