using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class LongExtensionTests
{
    [Theory]
    [InlineData(11999999999, "(11) 99999-9999")]
    [InlineData(1133334444, "(11) 3333-4444")]
    [InlineData(21987654321, "(21) 98765-4321")]
    public void ToPhoneNumber_ValidNumbers_ReturnsFormattedPhone(long input, string expected)
    {
        var result = input.ToPhoneNumber();

        result.Should().Be(expected);
    }

    [Fact]
    public void ToPhoneNumber_Zero_ReturnsNull()
    {
        var result = 0L.ToPhoneNumber();

        result.Should().BeNull();
    }

    [Fact]
    public void ToPhoneNumber_NegativeNumber_ReturnsNull()
    {
        var result = (-123456789L).ToPhoneNumber();

        result.Should().BeNull();
    }

    [Fact]
    public void ToPhoneNumber_Landline8Digits_FormatsWithHyphen()
    {
        var result = 1133334444L.ToPhoneNumber();

        result.Should().Be("(11) 3333-4444");
    }

    [Fact]
    public void ToPhoneNumber_Mobile9Digits_FormatsWithHyphen()
    {
        var result = 11999998888L.ToPhoneNumber();

        result.Should().Be("(11) 99999-8888");
    }
}
