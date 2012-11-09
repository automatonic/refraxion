using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Refraxion.Model;
using System.Reflection;
using System.IO;

namespace Refraxion.Test
{
    [TestClass]
    public class TestDataAssembly
    {
        [TestMethod]
        public void TestMethod1()
        {
            Compiler compiler = new Compiler();
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            compiler.InputAssemblyPaths.Add(Path.Combine(dir, "Refraxion.Test.Data.dll"));
            RxProjectInfo projectInfo = compiler.Compile();


            projectInfo.Build(compiler);
            projectInfo.Write(compiler);
        }
    }
}
