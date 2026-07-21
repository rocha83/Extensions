using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class StringExtensionTests
{
    // ------------------------------------------------------------------
    // ToNormalizedDescription
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToNormalizedDescription_NullOrEmpty_ReturnsEmpty(string? input)
    {
        var result = input.ToNormalizedDescription();

        result.Should().BeEmpty();
    }

    [Fact]
    public void ToNormalizedDescription_SingleLineWithColon_TrimsAndKeeps()
    {
        var input = "MARCA: MASSEY FERGUSON";

        var result = input.ToNormalizedDescription();

        result.Should().Contain("MARCA: MASSEY FERGUSON");
    }

    [Fact]
    public void ToNormalizedDescription_MultipleColonLines_AllFormatted()
    {
        var input = "MARCA: MASSEY FERGUSON\nMODELO: 292\nANO: 2008";

        var result = input.ToNormalizedDescription();

        result.Should().Contain("MARCA:");
        result.Should().Contain("MODELO:");
        result.Should().Contain("ANO:");
    }

    [Fact]
    public void ToNormalizedDescription_MixedColonAndNonColon_MergesLines()
    {
        var input = "MARCA:\nMASSEY FERGUSON\nMODELO:\n292";

        var result = input.ToNormalizedDescription();

        result.Should().Contain("MARCA:");
        result.Should().Contain("MASSEY");
        result.Should().Contain("MODELO:");
        result.Should().Contain("292");
    }

    [Fact]
    public void ToNormalizedDescription_RemovesEmptyLines()
    {
        var input = "LINHA1\n\n\nLINHA2\n\n";

        var result = input.ToNormalizedDescription();

        result.Should().NotContain("\n\n\n");
    }

    [Fact]
    public void ToNormalizedDescription_TrHandlesCarriageReturn()
    {
        var input = "MARCA: MASSEY\r\nMODELO: 292\r\nANO: 2008";

        var result = input.ToNormalizedDescription();

        result.Should().Contain("MARCA:");
        result.Should().NotContain("\r");
    }

    [Fact]
    public void ToNormalizedDescription_RealWorldExample_FormatsDescription()
    {
        var rawDescription = "TRATOR\r\nMARCA MASSEY FERGUSON\r\nMODELO 292 (4x4)\r\nANO 2008\r\nHORAS 9.000";

        var result = rawDescription.ToNormalizedDescription();

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("TRATOR");
        result.Should().Contain("MASSEY");
    }
}
