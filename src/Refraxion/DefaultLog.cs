using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Refraxion
{
    class DefaultLog : ILog
    {
        public void LogNormal(string format, params object[] parameters)
        {
            Console.WriteLine(format, parameters);
            Debug.WriteLine(format, parameters);
        }

        public void LogVerbose(string format, params object[] parameters)
        {
            Console.WriteLine(format, parameters);
            Debug.WriteLine(format, parameters);
        }

        public void LogWarning(string format, params object[] parameters)
        {
            Console.WriteLine(format, parameters);
            Debug.WriteLine(format, parameters);
        }

        public void LogError(string format, params object[] parameters)
        {
            Console.WriteLine(format, parameters);
            Debug.WriteLine(format, parameters);
        }

        public void LogException(Exception x)
        {
            Console.WriteLine(x.ToString());
            Debug.WriteLine(x.ToString());
        }
    }
}
