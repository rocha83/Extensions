using System.Text.Json;
using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class ObjectExtensionTests
{
    // ------------------------------------------------------------------
    // IsNumeric
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(123)]
    [InlineData(45.67)]
    [InlineData(-88.0)]
    [InlineData(0)]
    public void IsNumeric_NumericValues_ReturnsTrue(object value)
    {
        value.IsNumeric().Should().BeTrue();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    public void IsNumeric_NonNumericValues_ReturnsFalse(object value)
    {
        value.IsNumeric().Should().BeFalse();
    }

    // ------------------------------------------------------------------
    // IsMonetaryValue
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(10.50)]
    [InlineData("1.234,56")]
    public void IsMonetaryValue_MonetaryValues_ReturnsTrue(object value)
    {
        value.IsMonetaryValue().Should().BeTrue();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData(123)]
    public void IsMonetaryValue_NonMonetary_ReturnsFalse(object value)
    {
        value.IsMonetaryValue().Should().BeFalse();
    }

    // ------------------------------------------------------------------
    // ToJson
    // ------------------------------------------------------------------

    [Fact]
    public void ToJson_SimpleObject_ReturnsValidJson()
    {
        var obj = new { Name = "Teste", Value = 42 };

        var json = obj.ToJson();

        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"Name\"");
        json.Should().Contain("Teste");
        json.Should().Contain("42");
    }

    [Fact]
    public void ToJson_Collection_ReturnsJsonArray()
    {
        var list = new List<int> { 1, 2, 3 };

        var json = list.ToJson();

        json.Should().Be("[1,2,3]");
    }

    // ------------------------------------------------------------------
    // ToXML
    // ------------------------------------------------------------------

    [Fact]
    public void ToXML_SimpleObject_ReturnsValidXml()
    {
        var obj = new TestPayload { Name = "Teste", Value = 42 };

        var xml = obj.ToXML();

        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("<Name>Teste</Name>");
        xml.Should().Contain("<Value>42</Value>");
    }

    // ------------------------------------------------------------------
    // DumpToJson
    // ------------------------------------------------------------------

    [Fact]
    public void DumpToJson_ValidObject_WritesJsonFile()
    {
        var obj = new TestPayload { Name = "DumpTest", Value = 99 };
        var filePath = Path.Combine(Path.GetTempPath(), $"dump_test_{Guid.NewGuid()}.json");

        try
        {
            obj.DumpToJson(filePath);

            File.Exists(filePath).Should().BeTrue();
            var content = File.ReadAllText(filePath);
            content.Should().Contain("DumpTest");
            content.Should().Contain("99");
        }
        finally
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    // ------------------------------------------------------------------
    // DumpToXML
    // ------------------------------------------------------------------

    [Fact]
    public void DumpToXML_ValidObject_WritesXmlFile()
    {
        var obj = new TestPayload { Name = "XmlDump", Value = 77 };
        var filePath = Path.Combine(Path.GetTempPath(), $"dump_test_{Guid.NewGuid()}.xml");

        try
        {
            obj.DumpToXML(filePath);

            File.Exists(filePath).Should().BeTrue();
            var content = File.ReadAllText(filePath);
            content.Should().Contain("XmlDump");
        }
        finally
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    // ------------------------------------------------------------------
    // Clone
    // ------------------------------------------------------------------

    [Fact]
    public void Clone_SimpleObject_ReturnsDeepCopy()
    {
        var original = new TestPayload { Name = "Original", Value = 1 };

        var clone = original.Clone<TestPayload>();

        clone.Should().NotBeSameAs(original);
        clone.Name.Should().Be(original.Name);
        clone.Value.Should().Be(original.Value);
    }

    [Fact]
    public void Clone_ModifyClone_DoesNotAffectOriginal()
    {
        var original = new TestPayload { Name = "Original", Value = 1 };

        var clone = original.Clone<TestPayload>();
        clone.Name = "Modified";
        clone.Value = 999;

        original.Name.Should().Be("Original");
        original.Value.Should().Be(1);
    }

    // ------------------------------------------------------------------
    // ToHttpContent
    // ------------------------------------------------------------------

    [Fact]
    public void ToHttpContent_SimpleObject_ReturnsJsonContent()
    {
        var obj = new TestPayload { Name = "Http", Value = 55 };

        var content = obj.ToHttpContent();

        content.Should().NotBeNull();
        content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    // ------------------------------------------------------------------
    // GetDiff
    // ------------------------------------------------------------------

    [Fact]
    public void GetDiff_IdenticalObjects_ReturnsNull()
    {
        var obj1 = new TestPayload { Name = "Same", Value = 1 };
        var obj2 = new TestPayload { Name = "Same", Value = 1 };

        var diff = obj1.GetDiff(obj2);

        diff.Should().BeNull();
    }

    [Fact]
    public void GetDiff_DifferentObjects_ReturnsDifferences()
    {
        var obj1 = new TestPayload { Name = "Original", Value = 1 };
        var obj2 = new TestPayload { Name = "Changed", Value = 2 };

        var diff = obj1.GetDiff(obj2);

        diff.Should().NotBeNull();
    }

    [Fact]
    public void GetDiff_PartialChange_ReturnsOnlyChangedProps()
    {
        var obj1 = new TestPayload { Name = "Same", Value = 1 };
        var obj2 = new TestPayload { Name = "Same", Value = 99 };

        var diff = obj1.GetDiff(obj2);

        diff.Should().NotBeNull();
        var diffJson = JsonSerializer.Serialize(diff);
        diffJson.Should().Contain("Value");
        diffJson.Should().NotContain("Name");
    }

    // ------------------------------------------------------------------
    // Helper
    // ------------------------------------------------------------------

    public class TestPayload
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
