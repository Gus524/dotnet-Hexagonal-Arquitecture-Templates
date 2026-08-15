using FluentAssertions;
using SharedKernel.Events;
using Xunit;

namespace UnitTests.Domain.SharedKernel.Events;

public class DomainEventTests
{
    private record TestDomainEvent : DomainEvent;

    [Fact]
    public void Constructor_WhenInstantiated_ShouldAssignValidEventIdAndOccurredOn()
    {
        // Arrange
        var beforeInstantiation = DateTime.UtcNow;

        // Act
        var domainEvent = new TestDomainEvent();
        var afterInstantiation = DateTime.UtcNow;

        // Assert
        domainEvent.EventId.Should().NotBeEmpty();
        domainEvent.OccurredOn.Should().BeOnOrAfter(beforeInstantiation).And.BeOnOrBefore(afterInstantiation);
    }
}
