using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Utils
{
    public class Logger
    {
        public void Info(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
