using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class DecimalExtensionTests
{
    // ------------------------------------------------------------------
    // ToCultureString
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(1234.56, "en-US", "1,234.56")]
    [InlineData(1234.56, "pt-BR", "1.234,56")]
    [InlineData(1000000.00, "en-US", "1,000,000.00")]
    [InlineData(1000000.00, "pt-BR", "1.000.000,00")]
    [InlineData(0.99, "en-US", "0.99")]
    public void ToCultureString_VariousCultures_FormatsCorrectly(decimal value, string culture, string expected)
    {
        var result = value.ToCultureString(culture);

        result.Should().Be(expected);
    }

    // ------------------------------------------------------------------
    // ToInFullText
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(0.01, "UM CENTAVO")]
    [InlineData(0.50, "CINQUENTA CENTAVOS")]
    [InlineData(1.00, "UM REAL")]
    [InlineData(1.01, "UM REAL E UM CENTAVO")]
    [InlineData(10.00, "DEZ REAIS")]
    [InlineData(11.11, "ONZE REAIS E ONZE CENTAVOS")]
    [InlineData(100.00, "CEM REAIS")]
    [InlineData(150.50, "CENTO E CINQUENTA REAIS E CINQUENTA CENTAVOS")]
    [InlineData(200.00, "DUZENTOS REAIS")]
    [InlineData(1000.00, "UM MIL REAIS")]
    [InlineData(1500.00, "UM MIL E QUINHENTOS REAIS")]
    [InlineData(1000000.00, "UM MILHÃO DE REAIS")]
    public void ToInFullText_ValidValues_ReturnsCorrectText(decimal value, string expected)
    {
        var result = value.ToInFullText();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void ToInFullText_InvalidValues_ReturnsInvalidMessage(decimal value)
    {
        var result = value.ToInFullText();

        result.Should().Be("Invalid value informed.");
    }

    [Fact]
    public void ToInFullText_LargeValue_ReturnsTrilhoes()
    {
        var result = 1500000000000.00m.ToInFullText();

        result.Should().Contain("TRILH");
        result.Should().Contain("REAIS");
    }
}
