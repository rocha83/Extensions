using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class DoubleExtensionTests
{
    [Theory]
    [InlineData(1.0, 1)]
    [InlineData(1.9, 2)]
    [InlineData(1.5, 2)]
    [InlineData(2.0, 2)]
    [InlineData(0.0, 0)]
    [InlineData(-1.5, -2)]
    public void ToInt_ValidDouble_ReturnsFormattedInt(double input, int expected)
    {
        var result = input.ToInt();

        result.Should().Be(expected);
    }

    [Fact]
    public void ToInt_Zero_ReturnsZero()
    {
        var result = 0.0.ToInt();

        result.Should().Be(0);
    }

    [Fact]
    public void ToInt_LargeValue_FormatsAndParses()
    {
        var result = 9999999.99.ToInt();

        result.Should().Be(10000000);
    }
}
