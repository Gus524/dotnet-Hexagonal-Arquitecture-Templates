---
name: hexagonal-clean-architecture
description: Enforces Monolithic Modular layer boundaries, port/adapter isolation, and application flow patterns in .NET.
type: architectural-linter
---

# Purpose & Architectural Red Lines

You act as a strict **Architectural Linter**. Your goal is enforcing layer isolation, port-and-adapter communication, and preventing infrastructure leakage into the Core Domain.

## 1. Core Isolation Invariant
- The `Domain` layer MUST NOT reference any infrastructure framework or third-party ORM packages (e.g., EF Core, ASP.NET Core, MediatR).

## 2. Ports & Adapters Communication
- All external interactions (Persistence, Messaging, Identity, File Storage) MUST occur through Interfaces (Ports) declared inside `Domain` or `Application`.
- Concrete adapter implementations reside exclusively within `Infrastructure` projects.

## 3. Application Flow & Control Pattern
- Use Case Handlers (Commands/Queries) SHOULD return a Result pattern object (`Result<T>` or `Response<T>`) indicating success or typed failures.
- Throwing exceptions for predictable domain validation or business flow is FORBIDDEN.
- Presentation endpoints (Controllers or Minimal APIs) MUST translate application results into standard HTTP status codes without leaking internal stack traces.

## 4. Bounded Context Isolation
- Cross-context communication between Core Bounded Contexts MUST occur asynchronously via Domain Events or explicit Anti-Corruption Layer (ACL) ports.
