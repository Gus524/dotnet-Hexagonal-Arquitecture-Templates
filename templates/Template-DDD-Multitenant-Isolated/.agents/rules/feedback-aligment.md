---
trigger: always_on
---

# User Feedback Alignment & Dynamic Memory Protocol (.NET Backend)

## Core Mandate
You operate under a strict human-in-the-loop paradigm. Your primary goal is aligning your execution plans with the user's architectural preferences, stylistic choices, and C# / DDD patterns. You MUST prioritize architectural alignment and validation over raw execution speed.

## Protocols

### 1. Retrospective Initialization (The Memory Check)
- **Mandatory First Step:** At the start of any new session, task initialization, or subagent invocation, you MUST check the dynamic memory file located at `.agents/memory/lessons-learned.md` (if it exists).
- Analyze past architectural corrections, rejected anti-patterns, and direct instructions recorded there to ensure new proposals do not repeat past mistakes.

### 2. The Planning Gate (Architectural Blueprint & Execution Mode)
- **Direct Interactive Mode:** When receiving a new feature request, refactoring, or design task directly from the user, present a conceptual architectural blueprint (Domain Aggregates, Value Objects, Ports, Adapters, Layer Boundaries) and prompt the user for validation before modifying C# codebase files (`.cs`, `.csproj`).
- **Delegated & Automated Mode (Pre-Approved Tasks):** When invoked as a subagent or executing a task batch, specification, or plan that has already been approved by the user, proceed directly with C# code generation adhering strictly to the approved specification and project standards.

### 3. Dynamic Learning Protocol (Lessons Learned Updates)
- Whenever the user issues a strong correction, a structural critique, or points out a mistake in your C# code or architecture, activate the memory protocol immediately.
- **Action:** Append a new entry to `.agents/memory/lessons-learned.md`.
- **Entry Structure:**
  1. **Context/Module:** (e.g., Domain Model, EF Core Repository).
  2. **Rejected Anti-Pattern:** (e.g., Exposing public setters on Aggregate Root).
  3. **Corrected Behavior:** (e.g., Encapsulate mutation inside static factory and domain methods).
- Acknowledge this update to the user: *"I have recorded this lesson in `.agents/memory/lessons-learned.md` to avoid repeating this mistake."*

### 4. Code Generation Output Standard
- Once a blueprint or plan is ready for execution, cross-reference your C# output with `.agents/skills/hexagonal-clean-architecture.md` and `.agents/skills/ddd-technical-patterns.md` to guarantee structural and performant excellence in C#.

### 5. Multi-Agent & SDD Orchestrator Layer Binding
When executing under generic orchestrator phases (such as `sdd-apply`, `sdd-design`, or `sdd-verify`), the executing subagent MUST NOT generate or modify code for a specific layer until it has explicitly read its corresponding guidelines. 

You MUST use your file-reading tool (e.g., `view_file`) to load the following files based on the target path BEFORE writing any code:

1. **Domain Layer Files (`/Domain/`, `Aggregates`, `ValueObjects`, `Events`):** 
   - **ACTION:** Read `.agents/agents/tactical-ddd-modeler.md` and `.agents/skills/ddd-technical-patterns.md`. 
   - **FALLBACK RULE:** Public setters, mutable domain events, and primitive IDs are STRICTLY FORBIDDEN.

2. **Infrastructure Layer Files (`/Infrastructure/`, `Repositories`, `DbContexts`, `Adapters`):** 
   - **ACTION:** Read `.agents/agents/hexagonal-adapter-expert.md` and `.agents/skills/hexagonal-clean-architecture.md`.

3. **Testing Layer Files (`/UnitTests/`, `xUnit`, `Fixtures`):** 
   - **ACTION:** Read `.agents/agents/domain-quality-testing-expert.md` and `.agents/skills/dotnet-testing-strategy.md`.
