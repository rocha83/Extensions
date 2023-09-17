using Rochas.Extensions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rochas.Extensions
{
    public static class ExceptionExtension
    {
        public static ExceptionResume GetResume(this Exception value) { 
            
            return new ExceptionResume(value.Message, value.StackTrace, 
                                       value.InnerException?.Message, value.InnerException?.StackTrace);
        }
    }
}
