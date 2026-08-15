---
name: ci-cd-validation-flow
description: Mandatory automated test suites, build compilation, and SDD verification protocols for .NET Hexagonal Architecture.
domain: cross-cutting
---

# Mandatory Verification Commands

Before proposing code completion or marking tasks as finished, the agent MUST execute validation corresponding to the modified .NET projects.

## .NET Solution & Test Suite Validation
- **Build Compilation Check:**
  ```bash
  dotnet build --configuration Release
  ```
- **Context/Project Specific Tests:**
  ```bash
  dotnet test --filter "Category=[BOUNDED_CONTEXT_NAME]"
  ```
- **Global Solution Test Suite:**
  ```bash
  dotnet test
  ```

## Quality & Architecture Guardrails
- **Coverage Target:** Aim for 80%+ branch coverage for Domain invariants and Application use cases.
- **Zero Suppression:** Never ignore, comment out, or suppress failing unit/integration tests. If a test fails, diagnose the underlying contract breakage.
