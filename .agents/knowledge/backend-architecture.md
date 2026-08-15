---
name: backend-architecture
description: Layer topology, hexagonal boundaries, and modular architecture rules for .NET templates.
domain: backend
---

# Monolithic Modular & Hexagonal Topology

## Layer Isolation & Boundaries
Regardless of project organization (single solution with multiple `.csproj` libraries or folder-based modular organization), each Bounded Context MUST maintain strict onion boundaries:

1. **Domain (Core):** Aggregates, Entities, Value Objects, Domain Services, Domain Events, and Domain Port Interfaces. ZERO external framework dependencies.
2. **Application (Use Cases):** Command/Query Handlers, DTOs, Input Validation, Application Services, and Port Interfaces for external services.
3. **Infrastructure (Adapters Out):** EF Core DbContexts, Database Mappings, Repositories, External REST/gRPC API Clients, and Event Dispatchers.
4. **Presentation / WebApi (Adapters In):** ASP.NET Core Web API Controllers or Minimal APIs, Middleware, Swagger, and Dependency Injection wiring.

## Cross-Layer Dependencies Rule
- Dependencies flow **INWARD**: Presentation ➔ Infrastructure ➔ Application ➔ Domain.
- Domain MUST NOT depend on any outer layer. Infrastructure and Presentation implement interfaces defined in Domain and Application.
