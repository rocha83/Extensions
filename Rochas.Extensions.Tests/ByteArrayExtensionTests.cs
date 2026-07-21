using System.Text;
using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class ByteArrayExtensionTests
{
    // ------------------------------------------------------------------
    // ToNewString
    // ------------------------------------------------------------------

    [Fact]
    public void ToNewString_ValidUtf8Bytes_ReturnsDecodedString()
    {
        var original = "Olá, Mundo!";
        var bytes = Encoding.UTF8.GetBytes(original);

        var result = bytes.ToNewString();

        result.Should().Be(original);
    }

    [Fact]
    public void ToNewString_EmptyArray_ReturnsEmptyString()
    {
        var bytes = Array.Empty<byte>();

        var result = bytes.ToNewString();

        result.Should().BeEmpty();
    }

    [Fact]
    public void ToNewString_ByteArray_RoundTripsCorrectly()
    {
        var input = "Rochas Extensions \u00e9 legal!";
        var bytes = Encoding.UTF8.GetBytes(input);

        var result = bytes.ToNewString();

        result.Should().Be(input);
    }

    // ------------------------------------------------------------------
    // ToNewBase64String
    // ------------------------------------------------------------------

    [Fact]
    public void ToNewBase64String_SimpleText_ReturnsBase64()
    {
        var bytes = Encoding.UTF8.GetBytes("Hello");

        var result = bytes.ToNewBase64String();

        result.Should().Be("SGVsbG8=");
    }

    [Fact]
    public void ToNewBase64String_EmptyArray_ReturnsEmptyBase64()
    {
        var bytes = Array.Empty<byte>();

        var result = bytes.ToNewBase64String();

        result.Should().BeEmpty();
    }

    [Fact]
    public void ToNewBase64String_RoundTripsCorrectly()
    {
        var original = "Rochas Extensions 123!";
        var bytes = Encoding.UTF8.GetBytes(original);

        var base64 = bytes.ToNewBase64String();
        var decoded = Convert.FromBase64String(base64);
        var roundTrip = decoded.ToNewString();

        roundTrip.Should().Be(original);
    }

    // ------------------------------------------------------------------
    // ToNewHexString
    // ------------------------------------------------------------------

    [Fact]
    public void ToNewHexString_DefaultUpperCase_ReturnsUppercaseHex()
    {
        var bytes = new byte[] { 0xAB, 0xCD, 0xEF };

        var result = bytes.ToNewHexString();

        result.Should().Be("ABCDEF");
    }

    [Fact]
    public void ToNewHexString_LowerCase_ReturnsLowercaseHex()
    {
        var bytes = new byte[] { 0xAB, 0xCD, 0xEF };

        var result = bytes.ToNewHexString(lowerCase: true);

        result.Should().Be("abcdef");
    }

    [Fact]
    public void ToNewHexString_EmptyArray_ReturnsEmptyString()
    {
        var bytes = Array.Empty<byte>();

        var result = bytes.ToNewHexString();

        result.Should().BeEmpty();
    }

    [Fact]
    public void ToNewHexString_SingleByte_ReturnsTwoChars()
    {
        var bytes = new byte[] { 0x00 };

        var result = bytes.ToNewHexString();

        result.Should().Be("00");
    }

    [Fact]
    public void ToNewHexString_AllZeros_ReturnsCorrectLength()
    {
        var bytes = new byte[] { 0x00, 0x00, 0x00, 0x00 };

        var result = bytes.ToNewHexString();

        result.Should().HaveLength(8);
        result.Should().Be("00000000");
    }
}
