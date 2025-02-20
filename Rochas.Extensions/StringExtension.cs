using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using Rochas.Extensions.Helpers;

namespace Rochas.Extensions
{
    public static class StringExtension
    {
        public static object? FromJson(this string value)
        {
            return JsonSerializer.Deserialize<object>(value);
        }

        public static T FromJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        public static T FromXML<T>(this string value, string rootElem)
        {
            T result = default;

            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElem));

            using (var reader = new StringReader(value))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        public static byte[] ToByteArray(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }

        public static byte[] FromBase64String(this string value)
        {
            return Convert.FromBase64String(value);
        }

        public static string FilterSpecialChars(this string value)
        {
            int charCount = 0;
            var fromChars = "àáãçéíóõúÀÁÃÇÉÍÓÕÚ";
            var toChars = "aaaceioouAAACEIOOU";
            foreach (var character in fromChars)
                value = value.Replace(character, toChars[charCount++]);

            return value;
        }

        public static string ToNormalizedDescription(this string value)
        {
			var descArray = value.Split('\n').ToList();
			descArray.RemoveAll(ln => string.IsNullOrWhiteSpace(ln.Trim()));

			if (descArray.Any(ln => !ln.Contains(':')))
			{
				var count = 0;
				var newArray = descArray.ToList();
				foreach (var item in newArray)
				{
					var cleanedItem = item.Trim().Replace("\n", string.Empty);
					if (!cleanedItem.EndsWith(':'))
					{
						var descItem = descArray[count].Trim().Replace("\n", string.Empty);
						if (!cleanedItem.Contains(':'))
						{
							descArray[count] = $"{descItem} {cleanedItem}\n";
							descArray.Remove(item);
						}
						else
							descArray[count] = $"{cleanedItem}\n";

						count++;
					}
				}
			}
			else
			{
				var count = 0;
				foreach (var item in descArray)
				{
					descArray[count] = descArray[count].Trim().Replace("\n", string.Empty);
					descArray[count] += "\n";
				}
			}

			return string.Join('\n', descArray);
        }

        public static bool IsCPF(this string value)
        {
            int sum = 0;
            int[] multiplex1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplex2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            value = value.Trim().Replace(".", string.Empty).Replace(",", string.Empty).Replace("-", string.Empty);

            if (value.Length != 11)
                return false;

            var tempCPF = value.Substring(0, 9);

            for (int count = 0; count < 9; count++)
                sum += int.Parse(tempCPF[count].ToString()) * multiplex1[count];

            int rest = sum % 11;

            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            string digit = rest.ToString();

            tempCPF += digit;

            sum = 0;

            for (int count = 0; count < 10; count++)
                sum += int.Parse(tempCPF[count].ToString()) * multiplex2[count];

            rest = sum % 11;

            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            digit += rest.ToString();

            return value.EndsWith(digit);
        }

        public static string Zip(this string value)
        {
            return Compressor.ZipText(value);
        }

        public static string UnZip(this string value)
        {
            return Compressor.UnZipText(value);
        }
    }
}
