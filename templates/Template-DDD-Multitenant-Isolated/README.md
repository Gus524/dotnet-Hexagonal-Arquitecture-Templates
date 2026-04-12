# Backend (.NET 10) - Hexagonal Architecture

> [!IMPORTANT]
> This repository implements a backend using Hexagonal Architecture and schema-per-tenant multi-tenancy. The documentation is intended for backend developers who will maintain or extend the system.

---
**Summary**
- Hexagonal Architecture: domain-centric core, adapters for infrastructure and presentation.
- Multi-tenancy: isolated SQL schemas per tenant; one global Identity database.
- Core patterns implemented in-house: InternalMediator (mediator), Specification, IValidator and ValidationBehavior. No external libraries are used for these patterns.
---
**Solution overview**
1. Domain: pure business rules and entities (no external dependencies).
2. Application: use cases, commands/queries, DTOs and validators.
3. Infrastructure: adapters for persistence, identity, file storage, PDF generation, and shared services.
4. Presentation (WebApi): controllers, middleware, mediator pipeline.
---
**Key projects**
- WebApi: HTTP entry point, controllers, middlewares, InternalMediator.
- SharedKernel: common interfaces, Response\<T\>, ports and exceptions.
- Common: generic adapters and multi-tenant helpers (TenantRepositoryProxy, keyed services).
- Persistence: shared/global EF Core context, generic Repository and Specification implementations.
- Identity: ASP.NET Identity and JWT (global authentication).
- FileStorage: file storage providers and controllers.
- PdfGenerator: HTML/Razor to PDF generation service.
- Tenant adapters: one project per tenant with tenant DbContext and repositories.

**Developer conventions (short)**
- Project layout follows package-by-feature and vertical slices for features.
- Handlers return Response\<T\> to standardize success/error payloads.
- Use constructor injection for services and repositories.
- Register per-tenant services using keyed services (.NET 10); TenantRepositoryProxy routes IRepository\<TAggregate, TId\> calls to tenant-specific repositories.

Quick start
Requirements: .NET 10 SDK, SQL Server, VS/VSCode.

1. Restore packages: dotnet restore
2. Configure connection strings in appsettings.Development.json (see SETUP.md)
3. Apply migrations: dotnet ef database update --project Persistence --startup-project WebApi
4. Run: dotnet run --project WebApi

Support and checklist before PR
- dotnet build without errors
- dotnet test passes
- Use Response\<T\> wrapper in handlers
- Validators implemented and registered
- Controllers secured with [Authorize] when applicable
