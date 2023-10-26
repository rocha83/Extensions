using Rochas.Extensions.Helpers;
using System;
using System.IO;
using System.Linq;
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

        public static object? GetDiff(this object originalObj, object changedObj)
        {
            var diffProps = originalObj.GetType().GetProperties()
                                       .Where(prp => !(prp.GetValue(changedObj, null) == null)
                                                  && !prp.GetValue(changedObj, null).Equals(prp.GetValue(originalObj, null)));

            if (diffProps.Count() > 0)
            {
                var jsonObj = "{\n {0} \n}";
                var diffPropList = new StringBuilder();
                diffPropList.AppendLine();
                foreach (var prop in diffProps)
                {
                    var propValue = prop.GetValue(changedObj, null);

                    if (propValue != null)
                        diffPropList.AppendLine(string.Concat("\t\"", prop.Name, "\" : ",
                                                GetJSONValue(prop.PropertyType, propValue), ", "));
                }

                var diffPropStr = diffPropList.ToString();

                if (diffPropStr.Length > 4)
                {
                    jsonObj = jsonObj.Replace("{0}", diffPropStr.Substring(0, diffPropStr.Length - 4));
                    return JsonSerializer.Deserialize<object>(jsonObj);
                }
                else
                    return null;
            }
            else
                return null;
        }

        private static string? GetJSONValue(Type propType, object propValue)
        {
            string? typedValue = null;

            if (propValue.GetType().Name.Equals("DBNull")
                || propValue.GetType().Name.Equals("Null"))
                typedValue = null;
            else if (propType.Name.Equals("Int16") || propType.Name.Equals("Int32") || propType.Name.Equals("Int64")
                    || propType.Name.Equals("Decimal") || propType.Name.Equals("Float") || propType.Name.Equals("Double")
                    || propType.Name.Equals("Boolean"))
                typedValue = propValue.ToString().ToLower();
            else if (propValue.GetType().Name.Equals("String"))
                typedValue = string.Concat("\"", propValue.ToString(), "\"");
            else if (propValue.GetType().Name.Equals("DateTime"))
                typedValue = string.Concat("\"", ((DateTime)propValue).ToString("yyyy-MM-dd HH:mm:ss"), "\"");

            return typedValue;
        }
    }
}
