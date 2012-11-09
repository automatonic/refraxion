using System;
using System.Collections.Generic;
using System.Diagnostics;
using Refraxion.Model;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {
        public Compiler()
        {
            InputAssemblyPaths = new List<string>();
        }

        public string TypePageStylesheet { get; set; }

        public string AssemblyPageStylesheet { get; set; }

        public string NamespacePageStylesheet { get; set; }

        public string AssemblySidebarStylesheet { get; set; }

        public string NamespaceSidebarStylesheet { get; set; }

        public string TypeSidebarStylesheet { get; set; }

        public string OutputFolder { get; set; }

        public string OutputPath { get; set; }

        public List<string> InputAssemblyPaths { get; private set; }

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

        Stack<object> _STack;

        public RxProjectInfo Compile()
        {
            RxProjectInfo projectInfo = new RxProjectInfo();
            projectInfo.Build()


            return new RxProjectInfo();
        }
    }
}
