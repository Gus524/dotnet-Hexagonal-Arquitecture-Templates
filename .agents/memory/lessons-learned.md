# Lessons Learned & User Architectural Preferences

This file records user feedback, rejected anti-patterns, and architectural preferences to maintain long-term alignment across sessions.

---

### Domain Events Inheritance & Base Record Strategy
- **Context:** Core Domain Events.
- **Rejected Anti-Pattern:** Implementing `IDomainEvent` directly or adding redundant `DateTimeOffset OccurredOn` / `Guid EventId` parameters manually to concrete event record parameters.
- **Corrected Behavior:** ALL concrete domain events MUST inherit from the base abstract record `DomainEvent`, which automatically encapsulates `EventId` (`Guid.NewGuid()`) and `OccurredOn` (`DateTimeOffset.UtcNow`) initialization.

