namespace Cocktails.Api.Domain.Unit.Tests;

using Cezzi.Applications.Logging;
using FluentAssertions;
using Xunit;

public class MonikersTests
{
    [Fact]
    public void ServiceBus_ShouldReturnInstanceOfServiceBusMonikers()
    {
        // Arrange

        // Act
        var result = Monikers.ServiceBus;

        // Assert
        result.Should().BeOfType<ServiceBusMonikers>();
    }
}
