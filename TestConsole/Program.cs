using Rochas.Extensions;

var sampleSource = @"

using System.Linq;

public class FRTDescriptionNormalizer
{
    string descriptionTemplate = @""Marca: {0}

                        Modelo: {1}

                        Versão: {2}

                        Ano: {3}

                        Cor: {4}

                        Combustível: {5}

                        Categoria: CARROS

                        Tipo de Documento: NORMAL

                        Final da Placa: {6}

                        Chaves: SIM"";

    public string Normalize(string description)
    {
        var descriptionArray = description.Split("","");

        var productBranch = description.Substring(0, description.IndexOf(""/"")).Trim();
        var firstSpace = description.IndexOf("" "");
        var productModel = description.Substring(description.IndexOf(""/"") + 1, (firstSpace - productBranch.Length - 1)).Trim();
        var productVersion = description.Substring(description.IndexOf("" ""), 
                                                   description.IndexOf("","") - (productBranch.Length + (productModel.Length + 1))).Trim();
        var productYear = descriptionArray[1].Trim();
        var finalPlateNumber = descriptionArray[2].LastOrDefault().ToString();
        var productFuel = descriptionArray[3].Trim();
        var productColor = descriptionArray[4].Trim().Substring(0,
                            descriptionArray[descriptionArray.Length - 1].Trim().IndexOf("" ""));

        return string.Format(descriptionTemplate, productBranch, productModel, productVersion, productYear, productColor, productFuel, finalPlateNumber);
    }
}";

var originalDescription = @"FIAT/ARGO DRIVE 1.0, 20/20, PLACA: Q__-___6, GASOL/ALC, PRETA

                            FIAT/ARGO DRIVE 1.0, 20/20, PLACA: Q__-___6, GASOL/ALC, PRETA Visitação Virtual

                            IPVA 2025 PAGO";

var normalizedDescription = sampleSource.ExecuteAsSourceCode("FRTDescriptionNormalizer",
                                                             "Normalize", originalDescription) as string;

// Object Diff

var x1 = new BaseEntity()
{
    Id = 1,
    CreationDate = DateTime.Now,
    Name = "Original Data",
    Description = "Lorem Ipslum Lorem Ipslum Lorem Ipslum",
    Active = true
};

var x2 = new BaseEntity()
{
    Id = 1,
    CreationDate = DateTime.Now.AddDays(1),
    Name = "Modified Data",
    Description = "Lorem Ipslum Lorem Ipslum Lorem Ipslum",
    Active = false
};

var x3 = x1.GetDiff(x2);

Console.WriteLine(x3);

Console.WriteLine();
Console.WriteLine("---");
Console.WriteLine();

// String Normalization

var rawDescription = "TRATOR\r\nMARCA MASSEY FERGUSON\r\nMODELO 292 (4x4)\r\nANO 2008\r\nHORAS 9.000\r\nTRAÇÃO 4X4\r\nPOTENCIA 105 CV\r\nVALOR R$ 180.000,00";
normalizedDescription = rawDescription.ToNormalizedDescription();
Console.WriteLine(normalizedDescription);

Console.WriteLine();
Console.WriteLine("---");
Console.WriteLine();

// String Tokenization
var rawText = "As maiores bolsas de valores dos Estados Unidos começaram a semana em queda, com especial atenção voltada à Tesla de Elon Musk. As ações da montadora despencaram mais de 15% no pregão desta segunda-feira (10) e perderam toda a valorização obtida desde a eleição do presidente norte-americano, Donald Trump, no ano passado. Segundo a agência Reuters, a marca acumula a maior redução em seu valor de mercado entre as principais fabricantes de carros.\r\n\r\nA Tesla viu suas ações subirem nas primeiras semanas após a eleição de Donald Trump e chegou à marca de US$ 1,5 trilhão em valor de mercado em 17 de dezembro. De lá pra cá, no entanto, a companhia já registra uma queda de 52,4% em seu valor de mercado em três meses. Nesta segunda-feira (10), a Tesla fechou o pregão valendo US$ 714,6 bilhões. As ações de Elon Musk na política incluem demissões em massa de funcionários do governo dos EUA, enquanto o bilionário atua como assessor sênior do presidente americano. Investidores também estão preocupados que a política esteja distraindo o homem mais rico do mundo de sua principal fonte de receita.\r\n\r\nInvestidores seguem desapontados com Elon Musk\r\nO valor de mercado da Tesla não vem exclusivamente das vendas de carros elétricos. Segundo a Reuters, esse braço de negócios responde por cerca de 25%, com o restante baseado na promessa dos veículos totalmente autônomos, também chamados de robotáxis.\r\n\r\nEles ainda não foram lançados, mesmo tendo sido prometidos há quase uma década. O mais próximo que temos deste produto é uma apresentação de protótipo, feita em outubro de 2024, quando o próprio Elon Musk mostrou ao mundo o Cybercab.";
var textTokens = rawText.Tokenize();
Console.WriteLine($"Tokens of text: {rawText}");
Console.WriteLine();
foreach (var token in textTokens)
    Console.WriteLine(token);

Console.WriteLine();
Console.WriteLine("---");
Console.WriteLine();

Console.Read();

public class BaseEntity
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public DateTime CreationDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; }
}
