using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Refraxion.Model
{
    public partial class RxProjectInfo
    {
        [XmlIgnore]
        public SortedDictionary<string, RxMemberInfo> MemberLookup { get; private set; }

        [XmlIgnore]
        public HashSet<RxAssemblyInfo> Assemblies { get; private set; }

        [XmlIgnore]
        public HashSet<Assembly> ReferencedAssemblies { get; private set; }

        [XmlIgnore]
        public ReadOnlyCollection<RxMemberInfo> Members { get { return new ReadOnlyCollection<RxMemberInfo>(MemberLookup.Values.ToList()); } }

        public RxProjectInfo()
        {
            this.id = "P:Project";
            SetUri("P_Project");
            Assemblies = new HashSet<RxAssemblyInfo>(new RxMemberInfo.EqualityComparer());
            MemberLookup = new SortedDictionary<string, RxMemberInfo>();
            ReferencedAssemblies = new HashSet<Assembly>(new AssemblyEqualityComparer());
        }

        
        public void Write(Compiler context)
        {
            //Create our own namespaces for the output
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            //Add an empty namespace and empty value
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(RxProjectInfo));
            using (TextWriter writer = new StreamWriter(context.OutputPath))
            {
                serializer.Serialize(writer, this, ns);
            }
        }

        public bool TryGetMember(string id, out RxMemberInfo member)
        {
            return MemberLookup.TryGetValue(id, out member);
        }

        public void AddMember(RxMemberInfo member)
        {
            MemberLookup[member.id] = member;
        }

        internal Type PureGetType(string typeName)
        {
            Type t = Type.GetType(typeName);
            if (t != null)
                return t;

            foreach (Assembly a in ReferencedAssemblies)
            {
                t = a.GetType(typeName);
                if (t != null)
                    return t;
            }
            return null;
        }

        public Type GetType(Compiler context, string typeName)
        {
            //Detect Generic Type so we can mangle the name
            string mangled = RxTypeInfo.MangleGenericTypeNames(typeName);

            Type foundType = PureGetType(mangled);
            if (foundType != null)
                return foundType;

            string genericName = "N/A";

            int bracketLoc = mangled.IndexOf('[');
            if (bracketLoc >= 0)
            {
                genericName = mangled.Substring(0, bracketLoc);
                Type foundGeneric = PureGetType(genericName);
                if (foundGeneric != null)
                {
                    if (!foundGeneric.ContainsGenericParameters)
                    {
                        context.LogWarning("Expected an generic type with open parameters.\n\tGot: {0}", foundGeneric);
                    }
                    else
                    {
                        string parameters = mangled.Substring(bracketLoc + 1);
                        parameters = parameters.Substring(0, parameters.Length - 1);
                        List<Type> typeParameters = ParseTypeParameters(context, foundGeneric, parameters).ToList();
                        if (!typeParameters.Any(t => t.IsGenericParameterPlaceholder()))
                        {
                            if (typeParameters.Count == foundGeneric.GetGenericArguments().Length)
                            {
                                foundType = foundGeneric.MakeGenericType(typeParameters.ToArray());
                                if (foundType != null)
                                    return foundType;
                            }
                            context.LogWarning("Unable to build from generic type\n\tGeneric Type: {0}\n\tType Parameters:\n\t{1}", foundGeneric, typeParameters);
                        }
                        else
                            return foundGeneric;

                    }
                }
            }

            context.LogWarning("Cannot locate type.\n\tName: {0}\n\tMangled: {1}\n\tGeneric Type: {2}", typeName, mangled, genericName);

            return null;
        }

        public IEnumerable<Type> ParseTypeParameters(Compiler context, Type genericBase, string parameters)
        {
            string[] paramParts = parameters.CommaSplit();

            //Process each parameter individually
            foreach (string paramPart in paramParts)
            {
                if (paramPart.StartsWith("``") && genericBase != null)
                {
                    int pos;
                    if (!int.TryParse(paramPart.Substring(2), out pos))
                    {
                        context.LogWarning("Failed to resolve type parameter's runtime type\n\tParameter: {0}", paramPart.Substring(2));
                        yield break;
                    }
                    Type foundArgument = typeof(GenericArgumentPlaceholderHost<,,,,,,,,,,,,,>).GetGenericArguments()[pos];
                    yield return foundArgument;
                    continue;
                }
                Type paramType = GetType(context, paramPart);

                if (paramType != null)
                {
                    yield return paramType;
                }
                else
                {
                    context.LogWarning("Failed to resolve type parameter's runtime type\n\tParameter: {0}", paramPart);
                    yield break;
                }
            }
        }


        public void ResolveCrefs(string xapiPath)
        {
            XDocument source = XDocument.Load(xapiPath, LoadOptions.None);
            foreach (XElement element in source.XPathSelectElements("//*[@cref]"))
            {
                XAttribute crefAttribute = element.Attribute("cref");
                RxMemberInfo member = null;
                string cref = crefAttribute.Value;
                List<string> candidates = new List<string>();
                if (cref[1] != ':')
                {
                    candidates.Add(string.Concat("A:", cref));
                    candidates.Add(string.Concat("N:", cref));
                    candidates.Add(string.Concat("T:", cref));
                    candidates.Add(string.Concat("F:", cref));
                    candidates.Add(string.Concat("E:", cref));
                    candidates.Add(string.Concat("P:", cref));
                    candidates.Add(string.Concat("M:", cref));
                }
                else
                {
                    candidates.Add(cref);
                }

                foreach (string candidate in candidates)
                {
                    if (MemberLookup.TryGetValue(candidate, out member))
                    {
                        break;
                    }
                }

                if (member == null)
                {
                    string rawName = cref;
                    if (rawName[1] == ':')
                        rawName = rawName.Substring(2);
                    int lastDot = rawName.LastIndexOf('.');
                    member = new RxMemberLiteral();
                    member.SetUri(string.Format("http://msdn.microsoft.com/en-us/library/{0}.aspx", rawName));
                    member.id = cref;
                    member.caption = rawName.Substring(lastDot + 1);
                    MemberLookup[crefAttribute.Value] = member;
                }
                crefAttribute.Parent.SetAttributeValue("uri", member.AbsoluteUri);
                crefAttribute.Parent.SetAttributeValue("caption", member.caption);
            }
            source.Save(xapiPath);
        }

        public RxTypeInfo FindBuiltType(string id)
        {
            id = string.Concat("T:", id.Substring(2));
            int firstParen = id.IndexOf('(');
            if (firstParen >= 0)
            {
                id = id.Substring(0, firstParen);
            }
            while (true)
            {
                int lastDot = id.LastIndexOf('.');
                if (lastDot < 0)
                {
                    return null;
                }
                id = id.Substring(0, lastDot);
                RxMemberInfo member;
                if (TryGetMember(id, out member))
                {
                    return member as RxTypeInfo;
                }
            }
        }

        public class AssemblyEqualityComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                return x.GetName().Equals(y.GetName());
            }

            public int GetHashCode(Assembly obj)
            {
                return obj.GetName().GetHashCode();
            }
        }
    }
}
