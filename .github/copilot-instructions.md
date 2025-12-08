# eShopOnWeb Copilot Instructions

This repository follows a **Clean Architecture** (Monolithic) pattern with ASP.NET Core.

## Changelog
Please refer to `CHANGELOG.md` for the latest changes and updates to the project structure and infrastructure.

## Architecture Overview

- **src/ApplicationCore**: The "Core" of the application. Contains the Domain Model (Entities), Interfaces, and Business Logic. **No dependencies** on Infrastructure or Web.
- **src/Infrastructure**: Implementation of interfaces defined in Core. Includes Data Access (EF Core), Identity, Logging, and external service adapters.
- **src/Web**: The entry point. ASP.NET Core MVC/Razor Pages application. Depends on Core and Infrastructure.
- **src/BlazorAdmin**: Blazor WebAssembly application for administration.
- **src/PublicApi**: API endpoints for external consumption.

## Core Patterns & Conventions

### Data Access
- **Repository Pattern**: Do NOT use `DbContext` directly in the Web or Core layers. Use `IRepository<T>` (defined in `ApplicationCore`) for all data access.
  - Example: `private readonly IRepository<CatalogItem> _itemRepository;`
- **Specification Pattern**: Use `Ardalis.Specification` for querying. Do not write raw LINQ queries in services or controllers.
  - Define specifications in `src/ApplicationCore/Specifications`.
  - Example: `var spec = new CatalogFilterPaginatedSpecification(0, 10, typeId, brandId);`
  - Usage: `var items = await _itemRepository.ListAsync(spec);`

### Domain Modeling
- **Rich Domain Model**: Encapsulate business logic within Entities (`src/ApplicationCore/Entities`). Avoid "Anemic Domain Models" (entities with only public getters/setters and no logic).
- **Value Objects**: Use value objects for complex attributes that don't have identity.
- **Guard Clauses**: Use `Ardalis.GuardClauses` for input validation in constructors and methods.

### Web / UI
- **Razor Pages**: The main storefront uses Razor Pages (`src/Web/Pages`).
- **ViewModels**: Use ViewModels (in `src/Web/ViewModels`) to pass data to views. Do not pass Entities directly to views.
- **MediatR**: Used for handling requests/commands in some areas (check `src/Web/Features` if present, otherwise standard Service injection).

## Testing Strategy

- **Unit Tests** (`tests/UnitTests`): Test `ApplicationCore` logic (Entities, Specifications, Services). Mock dependencies.
- **Integration Tests** (`tests/IntegrationTests`): Test `Infrastructure` components (Repositories, EmailSender). Uses a real (in-memory or containerized) database.
- **Functional Tests** (`tests/FunctionalTests`): Test `Web` endpoints (Controllers, Pages). Simulates HTTP requests.
- **PublicApi Tests** (`tests/PublicApiIntegrationTests`): Test API endpoints.

## Development Workflow

### Build & Run
- **Web**: `dotnet run --project src/Web/Web.csproj`
- **Docker**: `docker-compose up`

### Database Migrations
- Migrations are in `src/Infrastructure/Data/Migrations`.
- To add a migration:
  ```powershell
  dotnet ef migrations add AddNewTable -p src/Infrastructure/Infrastructure.csproj -s src/Web/Web.csproj -c CatalogContext
  ```
- To update database:
  ```powershell
  dotnet ef database update -p src/Infrastructure/Infrastructure.csproj -s src/Web/Web.csproj -c CatalogContext
  ```

## Tech Stack
- **Framework**: .NET 9.0
- **ORM**: Entity Framework Core
- **Admin UI**: Blazor WebAssembly
- **Containerization**: Docker, Azure Container Apps
- **IaC**: Bicep (`infra/`)
