---
name: backend-architecture
description: Modular Monolith layer topology, physical project isolation per Bounded Context, and SharedKernel rules.
domain: backend
---

# Monolithic Modular Topology (Isolated BC Projects)

## Physical Project Boundaries & Strict Decoupling
Each Bounded Context (BC) MUST be structured as an independent C# Class Library (`.csproj`) to enforce physical compile-time boundary separation:

1. **Bounded Context Projects (`[ContextName].csproj`):** Houses the Domain Aggregates, Entities, Value Objects, Domain Events, Use Case Handlers, DTOs, and Port Interfaces.
2. **SharedKernel Project (`SharedKernel.csproj`):** Houses core domain abstractions (`AggregateRoot<TId>`, `Entity<TId>`, `ValueObject`, `DomainException`) and global Value Objects shared across Bounded Contexts.
3. **Infrastructure & Persistence Projects (`Identity.csproj`, `[Context].Infrastructure.csproj`):** Houses DbContexts, EF Core mappings, and outbound persistence adapters.
4. **Presentation Project (`WebApi.csproj`):** Houses ASP.NET Core Controllers/Minimal APIs, Middlewares, and Dependency Injection bootstrapping.

## Inter-Context Dependency Rule (Zero Direct BC References)
- **STRICT PROHIBITION:** A Bounded Context project MUST NEVER reference another Bounded Context project directly (`.csproj` to `.csproj` references between BCs are FORBIDDEN).
- All cross-context interactions MUST occur asynchronously via Domain Events or through contracts defined in `SharedKernel`.

## Layer Invariant
- Dependencies flow **INWARD**: Presentation ➔ Infrastructure ➔ Bounded Contexts ➔ SharedKernel.
- The Core Domain inside each BC MUST NOT reference Infrastructure, ASP.NET Core, or ORM packages.
