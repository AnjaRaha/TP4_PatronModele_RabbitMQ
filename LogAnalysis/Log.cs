using System;
using System.Collections.Generic;
using System.Text;

namespace LogAnalysis
{
   public class Log
    {
        public string severity;
        public string message;
        
        public Log(string severity, string message)
        {
            this.severity = severity;
            this.message = message;
        }


    }
}
