---
name: ubiquitous-language
description: Guidelines for establishing and enforcing canonical business glossary (Ubiquitous Language) in C# Domain models.
type: business-glossary
---

# Ubiquitous Language Enforcement Protocol

## 1. Domain Naming Consistency Mandate
- Domain aggregates, entities, value objects, and domain events MUST reflect the domain experts' ubiquitous language exactly.
- Avoid technical jargon in domain entity names (e.g., use `Order` or `Pedido` instead of `OrderEntity`, `OrderPOCO`, or `OrderTable`).

## 2. Business Language Selection Policy
- Choose **ONE primary language** (English or Spanish) for the Domain layer per Bounded Context and maintain it consistently across Class names, Property names, and Domain Events.
- Avoid mixing languages within the same Aggregate (e.g., avoid `Order.CalcularTotalAmount()`).

## 3. Domain Event Naming Policy
- Domain Events represent business facts that have **already occurred**.
- Name events cleanly in the **past tense** without technical suffixes like `Event` or `DomainEvent` (e.g., use `OrderPlaced`, `PedidoCreado`, `CustomerRegistered` instead of `OrderPlacedEvent` or `PedidoCreadoEvent`).

## 4. Consistency Guardrail
- Generic terms like `Data`, `Info`, `Manager`, `Processor`, or `Helper` MUST NOT be used for Domain Entities or Value Objects.
- If an unlisted business term is encountered during specification design, confirm naming with domain specifications before creating C# domain types.
