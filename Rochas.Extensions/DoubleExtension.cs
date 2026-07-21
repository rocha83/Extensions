using System;

namespace Rochas.Extensions
{
    public static class DoubleExtension
    {
        public static int ToInt(this double value)
        {
            return (int)Math.Round(value);
        }
    }
}
