---
name: bounded-context-map
description: Strategic domain categorization and inter-context communication guidelines for .NET applications.
domain: backend
---

# Strategic Bounded Context Mapping

## Subdomain Categorization Guidelines
When designing or extending backend templates, categorize domains according to DDD strategic design principles:

1. **Core Domains (High Business Value):** Primary competitive advantage of the system. Contains complex business rules and invariants.
2. **Supporting Subdomains:** Custom software built to support Core Domains (e.g., product catalog rules, member management).
3. **Generic Subdomains:** Standard off-the-shelf capabilities with no unique business logic (e.g., IAM, Notifications, File Storage).

## Inter-Context Communication & Sharing Rules
- **Asynchronous Decoupling:** Cross-context communication between Core Domains SHOULD occur asynchronously via **Domain Events** to maintain loose coupling.
- **Synchronous Integration (ACL):** When synchronous communication between contexts is required, calls MUST pass through an **Anti-Corruption Layer (ACL)**.
- **Shared Kernel Policy:** A **Shared Kernel** (`SharedKernel` / `Shared`) IS PERMITTED for common base abstractions (e.g., `AggregateRoot`, `DomainEvent`, `IDomainEventSubscriber`) and truly global Value Objects (e.g., `Money`, `Email`, `Percentage`) shared across Bounded Contexts. However, business Aggregates or mutable entities MUST NEVER be placed in the Shared Kernel.
