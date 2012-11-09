using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Refraxion.Model;
using System.Xml.Serialization;

namespace Refraxion
{
    /// <summary>
    /// Builds the Refraxion model up from documentation xml and .NET assemblies
    /// </summary>
    public partial class ModelBuilder
    {
        public ModelBuilder(string inputAssemblyPath)
            : this(null, null, new string[] { inputAssemblyPath })
        {
        }

        public ModelBuilder(string inputAssemblyPath, string outputPath)
            : this(null, outputPath, new string[] { inputAssemblyPath })
        {
        }

        public ModelBuilder(ILog log, string outputPath, string inputAssemblyPath)
            : this(log, outputPath, new string[] {inputAssemblyPath})
        {
        }

        public ModelBuilder(ILog log, string outputPath, params string[] inputAssemblyPaths)
            : this (log, outputPath, inputAssemblyPaths as IEnumerable<string>, null)
        {
        }

        public ModelBuilder(ILog log, string outputPath, IEnumerable<string> inputAssemblyPaths, IEnumerable<string> inputDocumenationFolders)
        {
            Log = log ?? new DefaultLog();
            HashSet<string> uniqueInputAssemblyPaths = new HashSet<string>();

            InputAssemblyPaths = (inputAssemblyPaths ?? new List<string>())
                .Distinct()
                .ToList();

            if (!InputAssemblyPaths.Any())
                throw new ArgumentOutOfRangeException("inputAssemblyPaths", "Need at least one assembly to process");

            foreach (string inputAssemblyPath in InputAssemblyPaths)
            {
                if (!File.Exists(inputAssemblyPath))
                    throw new FileNotFoundException(string.Format("Could not find assembly at \"{0}\"", inputAssemblyPath));
            }

            InputAssemblyFolders = InputAssemblyPaths.Select(path => Path.GetDirectoryName(path)).Distinct().ToList();

            InputDocumentationFolders = (inputDocumenationFolders ?? new List<string>()).Distinct().ToList();
            if (!InputDocumentationFolders.Any())
                InputDocumentationFolders.AddRange(InputAssemblyFolders);

            foreach (string inputDocFolder in InputDocumentationFolders)
            {
                if (!Directory.Exists(inputDocFolder))
                    throw new DirectoryNotFoundException(string.Format("Could not find documentation directory at \"{0}\"", inputDocFolder));
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                OutputPath = Path.ChangeExtension(InputAssemblyPaths.First(), ".rx");
            }
            else
            {
                OutputPath = outputPath;
            }

            OutputFolder = Path.GetDirectoryName(OutputPath);

            if (File.Exists(OutputFolder))
                throw new Exception(string.Format("Destination Folder is invalid: {0}", OutputFolder));

            if (!Directory.Exists(OutputFolder))
            {
                try
                {
                    Directory.CreateDirectory(OutputFolder);
                }
                catch (Exception x)
                {
                    throw new ArgumentException("Cannot create output directory", "outputPath", x);
                }
            }
        }

        public RxProjectInfo BuildProject()
        {
            RxProjectInfo info = new RxProjectInfo();
            info.id = "P:Project";
            info.SetUri("rx://P_Project");
            Log.LogNormal("Building Assembly Data");
            info.assembly = BuildProjectAssemblies(info).ToArray();
            
            Log.LogNormal("Consolidating Extension Methods");
            ILookup<string, MethodInfo> extensionMethodPartitions = info.assembly.SelectMany(assembly => assembly.ExtensionMethodLookup).ToLookup(pair => pair.Key, pair => pair.Value);
            foreach (var ambiguousMethodGroup in extensionMethodPartitions
                .Where(methods => methods.Count() > 1)
                .Select(record => record))
            {
                Log.LogWarning("Found {0} extension methods that mapped to the id {1}:", ambiguousMethodGroup.Count(), ambiguousMethodGroup.Key);
                foreach (var ambiguousMethod in ambiguousMethodGroup)
                {
                    Log.LogWarning("\t{0}", ambiguousMethod.ToString());
                }
            }

            Log.LogNormal("Consolidating Assembly types");

            info.@namespace = BuildProjectNamespaces(info).ToArray();


            

            return info;
        }

        

        public static void WriteProjectXml(RxProjectInfo projectInfo, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RxProjectInfo));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, projectInfo);
            }
        }

        public ILog Log { get; private set; }

        public string OutputPath { get; private set; }

        public string OutputFolder { get; private set; }

        public List<string> InputAssemblyPaths { get; private set; }

        public List<string> InputAssemblyFolders { get; private set; }

        public List<string> InputDocumentationFolders { get; private set; }
    }
}
