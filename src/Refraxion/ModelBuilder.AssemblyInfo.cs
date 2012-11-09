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
    public partial class ModelBuilder
    {
        Dictionary<string, MethodInfo> ExtensionMethodLookup = new Dictionary<string, MethodInfo>();

        IDictionary<string, XElement> memberElements;

        RxAssemblyInfo BuildProjectAssembly(RxProjectInfo parent, Assembly assembly, string assemblyCommentsPath)
        {
            RxAssemblyInfo info = new RxAssemblyInfo();

            info.Parent = parent;
            info.Assembly = assembly;
            info.id = string.Concat("A:", info.Assembly.GetName().Name);

            LoadReferencedAssemblies(info);
            Log.LogNormal("Built Assembly : {0}", info.id);
            info.SetUri(info.id.ToAbsoluteUri());
            info.caption = assembly.GetName().Name + ".dll";
            info.fileName = Path.GetFileName(assembly.Location);
            info.cultureInfo = assembly.GetName().CultureInfo.ToString();
            info.version = assembly.GetName().Version.ToString();
            BuildFileComments(info);

            BuildExtensionMethods(info);

            {
                //Load the input XML document
                XDocument source = XDocument.Load(assemblyCommentsPath, LoadOptions.None);

                //Build a dictionary based on the name/id of each member
                info.MemberXmlLookup = source.Descendants("member").ToDictionary(memberElement => memberElement.Attribute("name").Value);
                source = null;
            }

            //Partition types by namespace
            info.Namespaces = assembly.GetExportedTypes().Select(type => type.Namespace).Distinct().ToList();
            return info;
        }

        XElement GetCommentElement(string xid)
        {
            XElement ret = null;
            memberElements.TryGetValue(xid, out ret);
            if (ret == null)
            {
                Log.LogWarning("Could not find XML comments for \"{0}\"", xid);
            }
            return ret;
        }

        RxTypeInfo FindOrBuildXType(Type type, string xid)
        {
            RxMemberInfo memberInfo;
            RxTypeInfo typeInfo;
            if (TryGetMember(xid, out memberInfo))
                return (RxTypeInfo)memberInfo;

            XElement commentElement = GetCommentElement(xid);
            
            typeInfo = Build(type, commentElement, xid);
            AddMember(typeInfo);
            return typeInfo;
        }

        void BuildExtensionMethods(RxAssemblyInfo assemblyInfo)
        {
            assemblyInfo.ExtensionMethodLookup = GetExtensionMethods(assemblyInfo.Assembly)
                .ToDictionary(mi => 
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
                    return sb.ToString();
                });

                //MethodInfo existingExtensionMethod = null;
                //if (ExtensionMethodLookup.TryGetValue(sb.ToString(), out existingExtensionMethod))
                //{
                //    Log.LogWarning("Generated Extension Method Cref \"{0}\" is ambiguous\n\tExisting Method: \"{1}\"\n\tNew Method: \"{2}\"", sb.ToString(), existingExtensionMethod, mi);
                //}
                //else
                //{
                //    Log.LogVerbose("Found Extension Method \"{0}\"\n\tGenerated Cref:\"{1}\"", mi, sb);
                //    ExtensionMethodLookup.Add(sb.ToString(), mi);
                //}

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

        bool TryGetExtensionMethod(string id, out MethodInfo methodInfo)
        {
            return ExtensionMethodLookup.TryGetValue(id, out methodInfo);
        }
    }


}
