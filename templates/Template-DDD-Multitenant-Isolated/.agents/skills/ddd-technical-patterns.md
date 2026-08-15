---
name: ddd-technical-patterns
description: Technical C# implementation blueprints for Aggregate Roots, Value Objects, and Domain Events.
type: coding-blueprint
---

# Technical Blueprints in C#

## 1. Aggregate Root (Design by Contract)
Always enforce a **single private constructor** and **public static factory methods** to guarantee the aggregate enters the system in a valid state.

```csharp
namespace Domain.Aggregates;

public sealed class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    // Private Constructor: The sole state initializer
    private Order(OrderId id, CustomerId customerId, Money totalAmount) : base(id)
    {
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = OrderStatus.Created;
    }

    // Static Factory Method: Validates invariants and emits domain events
    public static Order Create(OrderId id, CustomerId customerId, Money totalAmount)
    {
        if (totalAmount.Amount <= 0)
            throw new DomainException("Order total amount must be positive.");

        var order = new Order(id, customerId, totalAmount);
        order.AddDomainEvent(new OrderPlaced(id, customerId, totalAmount));
        return order;
    }
}
```

## 2. Value Objects (Allocation & Performance Aware)
Choose the representation based on structural size and memory copy overhead:

- **Lightweight Value Objects & Single-Field IDs (<= 16–32 bytes):** Use `readonly record struct` (zero heap allocation) ONLY for single-field wrappers or small primitives (e.g., single `Guid`, `int`, `long`).
- **Composite or Large Value Objects (> 16–32 bytes or Multiple Fields):** Use `readonly record` (Reference type). DO NOT use `struct` for multi-field IDs or composite VOs, as copying large structs by value across stack frames degrades CPU cache and performance.

```csharp
// Lightweight ID (Single Guid <= 16 bytes) -> Zero heap allocation struct
public readonly record struct OrderId(Guid Value);

// Composite / Multi-field ID (> 16-32 bytes) -> Reference record to avoid stack copying overhead
public readonly record CompositeId(Guid TenantId, Guid EntityId, int Sequence);

// Composite Value Object -> Reference record
public readonly record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency = "USD") => new(0m, currency);
}
```

## 3. Domain Events
- Domain events represent immutable facts in the past tense without technical suffixes (`OrderPlaced`, `CustomerRegistered`).
- ALL concrete domain events MUST inherit from the base abstract record `DomainEvent`.
- The base `DomainEvent` automatically initializes `EventId` (`Guid.NewGuid()`) and `OccurredOn` (`DateTimeOffset.UtcNow`), avoiding redundant boilerplate in concrete events.

```csharp
// Base abstract record (defined in Domain/Seedwork or SharedKernel)
public abstract record DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

// Concrete Domain Event: Inherits base properties automatically
public sealed record OrderPlaced(
    OrderId OrderId, 
    CustomerId CustomerId, 
    Money TotalAmount
) : DomainEvent;
```
