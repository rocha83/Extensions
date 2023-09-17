using System;

namespace Rochas.Extensions
{
    public static class DoubleExtension
    {
        public static int ToInt(this double value)
        {
            int.TryParse(value.ToString("N0"), out var result);
            
            return result;
        }
    }
}
