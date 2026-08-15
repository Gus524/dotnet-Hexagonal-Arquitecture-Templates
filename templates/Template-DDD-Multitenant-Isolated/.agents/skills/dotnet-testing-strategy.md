---
name: dotnet-testing-strategy
description: Guidelines for xUnit, FluentAssertions, NSubstitute, BDD (Given-When-Then), Test Fixtures (Object Mother pattern), WebApplicationFactory integration tests, and targeted test execution in .NET.
type: testing-blueprint
---

# .NET Testing Strategy & Quality Enforcement

## 0. Mandatory Tech Stack & Mocking Policy
You MUST strictly use the following libraries. Do NOT mix testing frameworks or assertion styles:
- **Test Runner:** xUnit (`[Fact]`, `[Theory]`, `[InlineData]`)
- **Assertions:** FluentAssertions (`.Should().Be()`, `.Should().Throw<T>()`, `.Should().NotBeNull()`)
- **Mocking Policy (NSubstitute):** 
  - **Core Domain:** STRICTLY FORBIDDEN (0 Mocks). Use pure C# domain objects only.
  - **Application Use Cases:** PERMITTED ONLY for external adapter ports (e.g., `IEmailSender`, `IPaymentGateway`).
  - **Infrastructure Persistence:** STRICTLY FORBIDDEN. Use real test databases (In-Memory DB or Testcontainers).

## 1. Given-When-Then Structure (BDD Mandate)
Every test method MUST explicitly separate its execution phases using **Given-When-Then** comments:

```csharp
[Fact]
public void Given_InvalidNegativeAmount_When_CreatingOrder_Then_ShouldThrowDomainException()
{
    // Given
    var validCustomerId = CustomerId.New();
    var invalidAmount = Money.From(-100m, "USD");

    // When
    var action = () => Order.Create(OrderId.New(), validCustomerId, invalidAmount);

    // Then
    action.Should().Throw<DomainException>()
          .WithMessage("*amount must be positive*");
}
```

## 2. Test Fixtures & Object Mother Pattern (DRY Principle)
- **NO Inline Boilerplate Setup:** Avoid repeating inline instantiations of complex Aggregates or Value Objects across multiple tests.
- Use **Object Mother / Test Fixture factories** (`OrderFactory`, `CustomerBuilder`) to centralize valid default test data creation.

```csharp
// Centralized Test Fixture (Object Mother Pattern)
public static class OrderFactory
{
    public static Order CreateValidOrder(decimal amount = 100m)
    {
        return Order.Create(
            OrderId.New(), 
            CustomerId.New(), 
            Money.From(amount, "USD")
        );
    }
}
```

## 3. Integration Testing & WebApplicationFactory Pattern
- Infrastructure and WebAPI integration tests MUST derive from the template's pre-configured base test class (`CustomWebApplicationFactory` / `IntegrationTestBase`).
- This factory mirrors `Program.cs` and manages containerized or in-memory database instances automatically without database mocks.

```csharp
// Example: Integration Test inheriting from pre-configured WebApplicationFactory
public class CreateOrderControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateOrderControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
}
```

## 4. Targeted Test Execution Guidelines
- When executing tests for a specific change or Bounded Context, do NOT run the entire global test suite.
- Use filtered test execution commands targeted at the affected context or class:
  ```bash
  dotnet test --filter "FullyQualifiedName~Orders"
  ```

## 5. Boundary & Edge Case Stress-Testing
Always author test cases for:
- Null, empty string, or whitespace inputs.
- Empty GUIDs (`Guid.Empty`) in strongly-typed IDs.
- Boundary numbers (negative values, zero, overflow limits).
- Invalid domain state transitions.
