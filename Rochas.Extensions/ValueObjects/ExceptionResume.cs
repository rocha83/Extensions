using System;
using System.Collections.Generic;
using System.Text;

namespace Rochas.Extensions.ValueObjects
{
    public class ExceptionResume
    {
        public ExceptionResume(string message, string trace, string? childMessage = "", string? childTrace = "")
        {
            Message = message;
            Trace = trace;
            ChildMessage = childMessage;
            ChildTrace = childTrace;
        }
        public string Message { get; set; }
        public string Trace { get; set; }

        public string? ChildMessage { get; set; }

        public string? ChildTrace { get; set; }
    }
}
