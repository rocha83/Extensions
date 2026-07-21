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
    public void ToNormalizedDescription_RealWorldEquipment_FormatsAllLines()
    {
        var rawDescription = "CAMINHONETE\r\nMARCA CHEVROLET\r\nMODELO S10 4X4\r\nANO 2022\r\nKM 45.000\r\nCOMBUSTÍVEL DIESEL\r\nPOTENCIA 200 CV\r\nVALOR R$ 250.000,00";

        var result = rawDescription.ToNormalizedDescription();

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().NotContain("\r", "carriage returns devem ser removidos");

        var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(8, "todas as 8 linhas devem ser preservadas");
        lines[0].Trim().Should().Be("CAMINHONETE");
        lines[1].Trim().Should().Be("MARCA CHEVROLET");
        lines[2].Trim().Should().Be("MODELO S10 4X4");
        lines[3].Trim().Should().Be("ANO 2022");
        lines[4].Trim().Should().Be("KM 45.000");
        lines[5].Trim().Should().Be("COMBUSTÍVEL DIESEL");
        lines[6].Trim().Should().Be("POTENCIA 200 CV");
        lines[7].Trim().Should().Be("VALOR R$ 250.000,00");
    }
}
