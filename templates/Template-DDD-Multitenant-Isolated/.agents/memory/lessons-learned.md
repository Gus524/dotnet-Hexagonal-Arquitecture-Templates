# Lessons Learned & User Architectural Preferences

This file records user feedback, rejected anti-patterns, and architectural preferences to maintain long-term alignment across sessions.

---

### Domain Events Inheritance & Base Record Strategy
- **Context:** Core Domain Events.
- **Rejected Anti-Pattern:** Implementing `IDomainEvent` directly, adding redundant `DateTimeOffset OccurredOn` / `Guid EventId` parameters manually, or adding the suffix `Event` / `DomainEvent` to event names (e.g., `PrestatarioRegistradoEvent`).
- **Corrected Behavior:** ALL concrete domain events MUST inherit from the base abstract record `DomainEvent`, which automatically encapsulates `EventId` (`Guid.NewGuid()`) and `OccurredOn` (`DateTimeOffset.UtcNow`) initialization. Concrete event names MUST NOT include the suffix `Event` (use past tense domain verbs directly like `PrestatarioRegistrado`).


### EF Core Relational Dependencies & Entity Configuration
- **Context:** `Common` project entity configurations (Outbox / EventStore).
- **Rejected Anti-Pattern:** Creating extension methods on `ModelBuilder` without referencing `Microsoft.EntityFrameworkCore.Relational`, which causes missing definition errors for `ToTable` and `HasDefaultValue`.
- **Corrected Behavior:** Always include `<PackageReference Include="Microsoft.EntityFrameworkCore.Relational" />` in `Common.csproj` to support relational extension methods agnostically. Implement `IEntityTypeConfiguration<T>` in `Common/Configurations/` and load them via `modelBuilder.ApplyConfigurationsFromAssembly(...)`.

### xUnit Integration Testing (`IAsyncLifetime` vs `WebApplicationFactory`)
- **Context:** `IntegrationTests/Shared/WebAppFactory.cs` implementing `IAsyncLifetime` and inheriting from `WebApplicationFactory<Program>`.
- **Rejected Anti-Pattern:** Defining `ValueTask InitializeAsync()` and `override ValueTask DisposeAsync()` when using `xunit` 2.x.
### Bounded Context Domain Directory Structure
- **Context:** Domain layer organization across Bounded Contexts (`Prestamos`, `IAM`).
- **Rejected Anti-Pattern:** Using inconsistent directory names like `Domain/Entities/` in one context and `Domain/Model/` in another.
- **Corrected Behavior:** Standardize all Bounded Contexts to place domain entities, aggregate roots, and value objects under `Domain/Model/` and namespace `{BoundedContext}.Domain.Model`.

### Cross-Context Integration Handlers & Application Boundary
- **Context:** `EventsIntegration` project / Anti-Corruption Layer (ACL).
- **Rejected Anti-Pattern:** Influx integration handlers should not inject domain repositories (e.g., `IUsuarioRepository`) or mutate/create domain entities belonging to another Bounded Context directly within the infrastructure/integration layer. Furthermore, the subscriber should not generate temporary passwords or handle credential logic, nor should it wrap operations in redundant try/catch blocks.
- **Corrected Behavior:** Integration Handlers MUST NOT bypass the target context's Application layer. They MUST map integration events to Application Commands (e.g. `CreateUserCommand`) and dispatch them via `IMediator.Send(...)` / `ICommandHandler`. Password generation/credentials defaults belong inside `IAM.Application`. Exception handling and logging are handled transparently by `ExceptionHandlingDomainEventSubscriberDecorator<TEvent>`.






