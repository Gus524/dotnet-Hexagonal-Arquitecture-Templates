---
name: multi-tenancy-strategy
description: Architectural guidelines for multi-tenant isolation, tenant context resolution, and Strangler Fig legacy migration patterns in .NET.
domain: cross-cutting
---

# Multi-Tenancy & Legacy Migration Strategy

## 1. Multi-Tenant Architectural Models
When implementing multi-tenancy in Hexagonal Architecture templates, choose one of the following isolation strategies:
- **Database-per-Tenant (Isolated Persistence):** Each tenant routes to an independent physical database. Domain models remain 100% tenant-agnostic.
- **Schema-per-Tenant:** Tenants share the same database instance but occupy separate database schemas.
- **Discriminator Column (Shared Database):** Shared tables filtered via global query filters (`TenantId`).

## 2. Core Invariant (Tenant Purity)
- The **Core Domain MUST NEVER contain `switch(tenantId)` or tenant-specific conditionals**.
- Tenant differences in business rules must be handled via domain configuration objects (e.g., `TenantPolicy`, `ProductTemplate`) injected at runtime via ports.

## 3. Strangler Fig Migration Pattern (Higo Estrangulador)
When migrating legacy monoliths to this Hexagonal Architecture:
1. **Pilot Tenant Selection:** Identify a single tenant or low-risk bounded context to act as the primary target for initial deployment.
2. **Anti-Corruption Layer (ACL):** Intercept incoming calls at the API gateway or via ACL adapters, routing migrated features to the new .NET core while proxying unmigrated features back to the legacy system.
3. **Incremental Migration:** Gradually shift remaining tenants and domains until the legacy system is fully replaced and decommissioned.
