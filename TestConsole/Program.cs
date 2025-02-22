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

var rawDescription = "TRATOR\r\nMARCA MASSEY FERGUSON\r\nMODELO 292 (4x4)\r\nANO 2008\r\nHORAS 9.000\r\nTRAÇÃO 4X4\r\nPOTENCIA 105 CV\r\nVALOR R$180.000,00";
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
