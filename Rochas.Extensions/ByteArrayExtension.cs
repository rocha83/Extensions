using System;
using System.Text;

namespace Rochas.Extensions
{
    public static class ByteArrayExtension
    {
        public static string ToNewString(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        public static string ToNewBase64String(this byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        public static string ToNewHexString(this byte[] value, bool lowerCase = false)
        {
            StringBuilder result = new StringBuilder(value.Length * 2);

            for (int i = 0; i < value.Length; i++)
                result.Append(value[i].ToString(lowerCase ? "x2" : "X2"));

            return result.ToString();
        }
    }
}
