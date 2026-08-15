using FluentAssertions;
using IAM.Domain.Model;
using Xunit;

namespace UnitTests.Domain.IAM.ValueObjects;

public class RefreshTokenIdTests
{
    [Fact]
    public void New_ShouldGenerateNonEmptyGuidRefreshTokenId()
    {
        // Act
        var id = RefreshTokenId.New();

        // Assert
        id.Should().NotBeNull();
        id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Equality_TwoRefreshTokenIdsWithSameGuid_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = new RefreshTokenId(guid);
        var id2 = new RefreshTokenId(guid);

        // Act & Assert
        id1.Should().Be(id2);
    }

    [Fact]
    public void Equality_TwoRefreshTokenIdsWithDifferentGuids_ShouldNotBeEqual()
    {
        // Arrange
        var id1 = RefreshTokenId.New();
        var id2 = RefreshTokenId.New();

        // Act & Assert
        id1.Should().NotBe(id2);
    }
}
