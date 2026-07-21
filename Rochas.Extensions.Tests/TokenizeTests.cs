using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class TokenizeTests
{
    // ------------------------------------------------------------------
    // 1. Entradas nulas, vazias e whitespace
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    public void Tokenize_NullOrEmpty_ReturnsNull(string? input)
    {
        var result = input.Tokenize();
        result.Should().BeNull("entrada nula/vazia/whitespace deve retornar null");
    }

    // ------------------------------------------------------------------
    // 2. Palavra isolada sem stopwords nem caracteres especiais
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_SingleWord_ReturnsSingleToken()
    {
        var result = "lua".Tokenize();

        result.Should().NotBeNull();
        result!.Should().HaveCount(1);
        result[0].Should().Be("lua");
    }

    // ------------------------------------------------------------------
    // 3. Frase simples sem stopwords significativas
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_SimplePhrase_ReturnsAllMeaningfulTokens()
    {
        var result = "Apollo 11 lunar landing".Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("apollo");
        result.Should().Contain("11");
        result.Should().Contain("lunar");
        result.Should().Contain("landing");
    }

    // ------------------------------------------------------------------
    // 4. Remoção de stopwords portuguesas
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_PortugueseWithStopwords_RemovesConnectives()
    {
        var result = "a ida do homem a lua".Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("homem");
        result.Should().Contain("lua");
        result.Should().NotContain("do");
        result.Should().NotContain("a", "stopword 'a' deve ser removida");
    }

    [Fact]
    public void Tokenize_LongerPortugueseSentence_RemovesMultipleStopwords()
    {
        var result = "fale-me sobre a ida do homem a lua".Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("homem");
        result.Should().Contain("lua");
        result.Should().NotContain("sobre");
    }

    // ------------------------------------------------------------------
    // 5. Filtro de caracteres especiais
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_SpecialChars_AreStripped()
    {
        var result = "Olá, Mundo! Como vai?".Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("ola");
        result.Should().Contain("mundo");
        foreach (var token in result)
        {
            token.Should().NotContainAny(new[] { ",", "!", "?", ":", ";" },
                "caracteres especiais devem ser removidos");
        }
    }

    [Fact]
    public void Tokenize_DotsAndPipes_AreRemoved()
    {
        var result = "valor.total: R$100.00|desconto:10%".Tokenize();

        result.Should().NotBeNull();
        foreach (var token in result)
        {
            token.Should().NotContainAny(new[] { ".", ":", "|" },
                "pontos, dois-pontos e pipes devem ser removidos");
        }
    }

    // ------------------------------------------------------------------
    // 6. Normalização de acentos
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_AccentedChars_AreNormalized()
    {
        var result = "ação é fácil café".Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("acao");
        result.Should().Contain("facil");
        result.Should().Contain("cafe");
    }

    [Fact]
    public void Tokenize_UpperCaseAccented_NormalizedAndLowercased()
    {
        var result = "TRATOR MARCA MASSEY FERGUSON".Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("trator");
        result.Should().Contain("marca");
        result.Should().Contain("massey");
        result.Should().Contain("ferguson");
    }

    // ------------------------------------------------------------------
    // 7. Hífens isolados
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_StandaloneHyphen_IsFiltered()
    {
        var result = "teste - palavra".Tokenize();

        result.Should().NotBeNull();
        result!.Should().NotContain("-");
        result!.Should().Contain("teste");
        result.Should().Contain("palavra");
    }

    [Fact]
    public void Tokenize_HyphenInsideWord_Preserved()
    {
        var result = "fale-me".Tokenize();

        result.Should().NotBeNull();
    }

    // ------------------------------------------------------------------
    // 8. Texto com quebras de linha
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_MultilineText_HandledCorrectly()
    {
        var input = "TRATOR\r\nMARCA MASSEY FERGUSON\r\nMODELO 292 (4x4)\r\nANO 2008";
        var result = input.Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("trator");
        result.Should().Contain("massey");
        result.Should().Contain("ferguson");
        result.Should().Contain("modelo");
        result.Should().Contain("292");
        result.Should().Contain("4x4");
        result.Should().Contain("2008");
    }

    // ------------------------------------------------------------------
    // 9. Texto longo real (simula cenário de uso do sgev3)
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_LongRealText_ReturnsNonEmptyTokens()
    {
        var rawText = "As maiores bolsas de valores dos Estados Unidos começaram a semana em queda, " +
            "com especial atenção voltada à Tesla de Elon Musk. As ações da montadora despencaram " +
            "mais de 15% no pregão desta segunda-feira (10) e perderam toda a valorização obtida " +
            "desde a eleição do presidente norte-americano, Donald Trump, no ano passado.";

        var result = rawText.Tokenize();

        result.Should().NotBeNull();
        result!.Length.Should().BeGreaterThan(0);
        result.Should().Contain("tesla");
        result.Should().Contain("bolsas");
        result.Should().Contain("presidente");
        result.Should().Contain("trump");
    }

    // ------------------------------------------------------------------
    // 10. Validação de integração: cenário sgev3 (BuildTags)
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_Sgev3UserMessage_ExtractsKeywordsForTags()
    {
        var userMessage = "voce conhece e sabe explicar sobre o projeto Apollo da NASA";
        var tokens = userMessage.Tokenize();

        tokens.Should().NotBeNull();
        tokens!.Should().Contain("projeto");
        tokens.Should().Contain("apollo");
        tokens.Should().Contain("nasa");
    }

    // ------------------------------------------------------------------
    // 11. Tokens não devem conter caracteres especiais residuais
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_NoTokenContainsSpecialCharacters()
    {
        var input = "Relatório: (vendas) R$1.500,00; Lucro: 12% [meta atingida]";
        var tokens = input.Tokenize();

        tokens.Should().NotBeNull();
        foreach (var token in tokens!)
        {
            token.Should().NotContainAny(new[] { ".", ",", ":", ";", "!", "?", "(", ")", "[", "]" },
                $"token '{token}' não deve conter caracteres especiais residuais");
        }
    }

    // ------------------------------------------------------------------
    // 12. Tokens não devem ser vazios ou whitespace
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_NoEmptyOrWhitespaceTokens()
    {
        var input = "  valor   do   produto   é   alto  ";
        var tokens = input.Tokenize();

        tokens.Should().NotBeNull();
        foreach (var token in tokens!)
        {
            token.Should().NotBeNullOrWhiteSpace();
            token.Trim().Should().NotBeEmpty();
        }
    }

    // ------------------------------------------------------------------
    // 13. Números são preservados
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_NumbersArePreserved()
    {
        var result = "ano 2008 horas 9000 valor 180000".Tokenize();

        result.Should().NotBeNull();
        result!.Should().Contain("2008");
        result.Should().Contain("9000");
        result.Should().Contain("180000");
    }

    // ------------------------------------------------------------------
    // 14. Stopwords compostas (multi-palavra) são removidas
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_CompositeStopwords_AreRemoved()
    {
        var result = "isto é por exemplo um teste".Tokenize();

        result.Should().NotBeNull();
        result!.Should().NotContain("por");
        result.Should().NotContain("exemplo");
        result.Should().NotContain("isto");
        result.Should().NotContain("e");
        result.Should().Contain("teste");
    }

    // ------------------------------------------------------------------
    // 15. Texto só com stopwords retorna vazio
    // ------------------------------------------------------------------

    [Fact]
    public void Tokenize_OnlyStopwords_ReturnsEmptyArray()
    {
        var result = "o a da de em no na".Tokenize();

        result.Should().NotBeNull();
        result!.Should().BeEmpty("somente stopwords não devem gerar tokens");
    }
}
