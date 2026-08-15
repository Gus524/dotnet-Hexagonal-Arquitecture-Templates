---
name: tech-stack-whitelist
description: Authorized runtime environments, approved NuGet packages, and layer dependency boundaries for .NET templates.
domain: backend
---

# Approved Technical Stack & Layer Dependencies

## Core Domain Layer (Zero-Dependency Zone)
- **Allowed Dependencies (Whitelist):**
  - `Microsoft.Extensions.Logging.Abstractions`
  - `Microsoft.Extensions.DependencyInjection.Abstractions`
- **Strictly Forbidden in Domain:**
  - Entity Framework Core (`Microsoft.EntityFrameworkCore`)
  - ASP.NET Core (`Microsoft.AspNetCore.*`)
  - Object-Relational Mappers or serialization libraries (Newtonsoft.Json, System.Text.Json)
  - MediatR or external mediator libraries (Domain must remain pure C#)

## Application Layer (Use Cases & Ports)
- **Allowed Dependencies:**
  - Domain Layer projects
  - `FluentValidation` (for input DTO validation)
  - `Microsoft.Extensions.Logging.Abstractions`

## Infrastructure & Adapters Layer (Implementations)
- **Allowed Dependencies:**
  - Application & Domain Layer projects
  - Entity Framework Core (`Microsoft.EntityFrameworkCore`, SQL Server / PostgreSQL providers)
  - Dapper / Refit / MassTransit / RabbitMQ (as needed by concrete adapters)
