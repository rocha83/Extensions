using Rochas.Extensions;

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

var rawDescription = "Marca: HARLEY-DAVIDSON\r\n\r\nModelo: SPORTSTER\r\n\r\nVersão: SPORTSTER XL 1200X - FORTY-EIGHT\r\n\r\nAno de Fabricação: 2017\r\n\r\nAno Modelo: 2018\r\n\r\nBlindado: Não\r\n\r\nTipo de Documento: Normal\r\n\r\nComitente: PREVISUL\r\n\r\nTipo de Monta: Não Aplicável\r\n\r\nCondição: FINANCIAMENTO\r\n\r\nValor FIPE: R$ 51.270,00 BRL\r\n\r\nChassi: \r\n\r\nTipo de Chassi: Normal\r\n\r\nCategoria: Motos\r\n\r\nCondição de Func.: Motor dá partida e engrena\r\n\r\nFinal de Placa: 6\r\n\r\nCombustível: Gasolina\r\n\r\nChave:Sim";
var normalizedDescription = rawDescription.ToNormalizedDescription();
Console.WriteLine(normalizedDescription);

Console.WriteLine();
Console.WriteLine("---");
Console.WriteLine();

Console.Read();

public class BaseEntity
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public DateTime CreationDate { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
}
