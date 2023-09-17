using System;
using System.Collections.Generic;
using System.Text;

namespace Rochas.Extensions
{
    public static class LongExtension
    {
        public static string? ToPhoneNumber(this long value)
        {
            if (value > 0)
            {
                if (value <= 9999999999)
                    return value.ToString(@"(00) 0000-0000");
                else
                    return value.ToString(@"(00) 00000-0000");
            }
            else
                return null;
        }
    }
}
