using ByCoders.CNAB.Infrastructure.Correlation;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Infrastructure;

public class CorrelationIdTests
{
    [Fact]
    public void SetCorrelationId_WhenCalled_ShouldStoreCorrectValue()
    {
        // Arrange
        var correlationId = new CorrelationId();
        var expectedId = Guid.NewGuid();

        // Act
        correlationId.SetCorrelationId(expectedId);
        var result = correlationId.GetCorrelationId();

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void GetCorrelationId_WhenNotSet_ShouldReturnEmptyGuid()
    {
        // Arrange
        var correlationId = new CorrelationId();

        // Act
        var result = correlationId.GetCorrelationId();

        // Assert
        result.Should().Be(Guid.Empty);
    }

    [Fact]
    public void SetCorrelationId_WhenCalledMultipleTimes_ShouldUpdateValue()
    {
        // Arrange
        var correlationId = new CorrelationId();
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        // Act
        correlationId.SetCorrelationId(firstId);
        var firstResult = correlationId.GetCorrelationId();
        
        correlationId.SetCorrelationId(secondId);
        var secondResult = correlationId.GetCorrelationId();

        // Assert
        firstResult.Should().Be(firstId);
        secondResult.Should().Be(secondId);
        secondResult.Should().NotBe(firstId);
    }

    [Fact]
    public void CorrelationId_AsRecord_ShouldSupportValueEquality()
    {
        // Arrange
        var id1 = new CorrelationId();
        var id2 = new CorrelationId();
        var sharedGuid = Guid.NewGuid();

        // Act
        id1.SetCorrelationId(sharedGuid);
        id2.SetCorrelationId(sharedGuid);

        // Assert
        id1.Should().NotBeSameAs(id2); // Different instances
        id1.GetCorrelationId().Should().Be(id2.GetCorrelationId());
    }

    [Fact]
    public void SetCorrelationId_WithEmptyGuid_ShouldStoreEmptyGuid()
    {
        // Arrange
        var correlationId = new CorrelationId();

        // Act
        correlationId.SetCorrelationId(Guid.Empty);
        var result = correlationId.GetCorrelationId();

        // Assert
        result.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CorrelationId_ImplementsICorrelation()
    {
        // Arrange
        var correlationId = new CorrelationId();

        // Act & Assert
        correlationId.Should().BeAssignableTo<ICorrelation>();
    }
}
