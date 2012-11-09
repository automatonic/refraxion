using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Refraxion.Model;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {

        public RxAssemblyInfo BuildAssemblyInfo(RxProjectInfo project, Assembly reflection, string assemblyCommentsPath)
        {
            RxAssemblyInfo instance = new RxAssemblyInfo()
            {
                Project = project,
                Reflection = reflection,
                id = string.Concat("A:", reflection.GetName().Name),
                caption = reflection.GetName().Name + ".dll",
                fileName = Path.GetFileName(reflection.Location),
                cultureInfo = reflection.GetName().CultureInfo.ToString(),
                version = reflection.GetName().Version.ToString(),
            };
            instance.SetUri(project, instance.id);


            project.AppendAssembly(context, this);
            context.LogNormal("Built Assembly : {0}", id);
            
            
            
            BuildFileComments(instance);

            BuildExtensionMethods(context);

            Project.AddMember(this);

            //Load the input XML document
            XDocument source = XDocument.Load(assemblyCommentsPath, LoadOptions.None);

            //Build a dictionary based on the name/id of each member
            memberElements = source.Descendants("member").ToDictionary(memberElement => memberElement.Attribute("name").Value);

            //Partition types by namespace
            ILookup<string, Type> typesByNamespace = reflection.GetExportedTypes().ToLookup(t => t.Namespace);

            List<RxNamespaceInfo> rxNamespaces = new List<RxNamespaceInfo>(typesByNamespace.Count);

            //Process each namespace
            foreach (IGrouping<string, Type> typeGrouping in typesByNamespace)
            {
                RxNamespaceInfo xns = new RxNamespaceInfo();
                xns.Build(context, this, typeGrouping.Key, typeGrouping);

                rxNamespaces.Add(xns);
            }
            this.@namespace = rxNamespaces.ToArray();
        }

        public XElement GetCommentElement(Compiler context, string xid)
        {
            XElement ret = null;
            memberElements.TryGetValue(xid, out ret);
            if (ret == null)
            {
                context.LogWarning("Could not find XML comments for \"{0}\"", xid);
            }
            return ret;
        }

        public RxTypeInfo FindOrBuildXType(Compiler context, Type type, string xid)
        {
            RxMemberInfo xMember;
            RxTypeInfo xType;
            if (Project.TryGetMember(xid, out xMember) && (xType = xMember as RxTypeInfo) != null)
                return xType;

            XElement commentElement = GetCommentElement(context, xid);
            xType = new RxTypeInfo();
            xType.Build(context, this, type, commentElement, xid);
            Project.AddMember(xType);
            return xType;
        }

        public IEnumerable<RxTypeInfo> GetTypes()
        {
            return Project.Members.OfType<RxTypeInfo>().Where(xt => xt.Assembly.id == id);
        }

        public void BuildExtensionMethods(Compiler context)
        {
            List<MethodInfo> extensionMethods = GetExtensionMethods(InputAssembly).ToList();
            ExtensionMethodLookup = new Dictionary<string, MethodInfo>(extensionMethods.Count);

            foreach (MethodInfo mi in extensionMethods)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("M:{0}.{1}", mi.DeclaringType.FullName, mi.Name);
                if (mi.IsGenericMethodDefinition)
                    sb.Append("`");
                Type[] genericMethodArguments = mi.GetGenericArguments();
                if (genericMethodArguments != null && genericMethodArguments.Length > 0)
                    sb.AppendFormat("`{0}", genericMethodArguments.Length);
                ParameterInfo[] parameters = mi.GetParameters();
                if (parameters != null && parameters.Length > 0)
                {
                    sb.Append("(");
                    sb.Append(string.Join(",", parameters.Select(p => p.ParameterType).Select(pt => pt.ToCref()).ToArray()));
                    sb.Append(")");
                }
                MethodInfo existingExtensionMethod = null;
                if (ExtensionMethodLookup.TryGetValue(sb.ToString(), out existingExtensionMethod))
                {
                    context.LogWarning("Generated Extension Method Cref \"{0}\" is ambiguous\n\tExisting Method: \"{1}\"\n\tNew Method: \"{2}\"", sb.ToString(), existingExtensionMethod, mi);
                }
                else
                {
                    context.LogVerbose("Found Extension Method \"{0}\"\n\tGenerated Cref:\"{1}\"", mi, sb);
                    ExtensionMethodLookup.Add(sb.ToString(), mi);
                }

            }
        }



        static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {

            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;

            return query;
        }

        static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly)
        {

            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        select method;

            return query;
        }

        public bool TryGetExtensionMethod(string id, out MethodInfo methodInfo)
        {
            return ExtensionMethodLookup.TryGetValue(id, out methodInfo);
        }
    }


}
