using Rochas.Extensions.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Rochas.Extensions
{
    public static class ObjectExtension
    {
        public static bool IsNumeric(this object value)
        {
            return double.TryParse(value.ToString(), out _);
        }

        public static bool IsMonetaryValue(this object value)
        {
            return IsNumeric(value) 
                && (value.ToString().Equals("0") 
                || value.ToString().Contains(".") 
                || value.ToString().Contains(","));
        }

        public static string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value);
        }

        public static string ToXML(this object value)
        {
            StringBuilder resultText = new StringBuilder();
            XmlWriter xmlGen = XmlWriter.Create(resultText);

            var serializer = new XmlSerializer(value.GetType());

            serializer.Serialize(xmlGen, value);

            return resultText.ToString();
        }

        public static void DumpToJson(this object value, string filePath)
        {
            var jsonContent = JsonSerializer.Serialize(value);
            File.WriteAllText(filePath, jsonContent);
        }

        public static void DumpToXML(this object value, string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(value.GetType());

            using var xmlWriter = XmlWriter.Create(filePath);
            xmlSerializer.Serialize(xmlWriter, value);
        }

        public static T Clone<T>(this object value)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value));
        }

        public static HttpContent ToHttpContent(this object value)
        {
            return new JsonContent(value);
        }
    }
}
