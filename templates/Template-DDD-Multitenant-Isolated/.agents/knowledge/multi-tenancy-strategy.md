---
name: multi-tenancy-strategy
description: Isolated persistence strategy (Database-per-Tenant), tenant routing resolution, and domain purity rules.
domain: cross-cutting
---

# Multi-Tenancy Architecture (Isolated Database Model)

## 1. Physical Isolation Model (Database-per-Tenant)
This architecture strictly implements **Database-per-Tenant (Isolated Persistence)**:
- Each organizational tenant routes to an independent physical database instance.
- Persistence repositories dynamically resolve connection strings per request via the tenant context provider.

## 2. Core Domain Purity (Zero TenantId Leaks)
- **Tenant Purity Invariant:** Core Domain Aggregates, Entities, and Value Objects **MUST NEVER contain `TenantId` fields or `switch(tenantId)` branching**.
- Multi-tenancy is completely transparent to the Core Domain layer. Domain logic executes identically regardless of which physical tenant database is targeted.

## 3. Tenant Context Resolution (WebApi / Infrastructure)
- Tenant identity is resolved at the API boundary (e.g., HTTP Header `X-Tenant-ID`) via middleware/services in the `WebApi` layer.
- Persistence adapters in Infrastructure consume the tenant provider to route DbContext connections dynamically.
