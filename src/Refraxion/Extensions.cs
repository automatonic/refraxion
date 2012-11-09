using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Refraxion.Model;
using System.Reflection;
using System.Xml.Linq;

namespace Refraxion
{
    public static class Extensions
    {
        public static string PagePrefix = null;

        public static string ToCaption(this string value)
        {
            return value.Replace("Void", "void");
        }

        public static string ToCaption(this MethodInfo methodInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(methodInfo.ReturnType.ToCaption());
            sb.Append(" ");
            sb.Append(methodInfo.Name);

            if (methodInfo.GetGenericArguments().Any())
            {
                sb.Append("&lt;");
                sb.Append(string.Join(", ", methodInfo.GetGenericArguments().Select(g => g.ToCaption()).ToArray()));
                sb.Append("&gt;");
            }
            sb.Append("(");
            if (methodInfo.GetParameters().Any())
            {
                sb.Append(string.Join(", ", methodInfo.GetParameters().Select(p => string.Concat(p.ParameterType.ToCaption(), " ", p.Name)).ToArray()));
            }
            sb.Append(")");
            return sb.ToString();
        }


        static SortedDictionary<string, string> _TypeCaptionLookup = new SortedDictionary<string, string>() 
        {
            {"System.Boolean","bool"},
            {"System.String","string"},
            {"System.Int16","short"},
            {"System.Int32","int"},
            {"System.Int64","long"},
            {"System.Double","bool"},
            {"System.Single","float"},
            {"System.Byte","byte"},
            {"System.Char","char"},
            {"System.Void","void"},
            {"System.Object","object"},
            
            {"System.Boolean[]","bool[]"},
            {"System.String[]","string[]"},
            {"System.Int16[]","short[]"},
            {"System.Int32[]","int[]"},
            {"System.Int64[]","long[]"},
            {"System.Double[]","bool[]"},
            {"System.Single[]","float[]"},
            {"System.Byte[]","byte[]"},
            {"System.Char[]","char[]"},
        };

        public static string ToCaption(this Type type)
        {
            string caption = null;
            if (!_TypeCaptionLookup.TryGetValue(type.FullName ?? type.Name, out caption))
            {
                caption = type._ToCaption();
                _TypeCaptionLookup[type.FullName ?? type.Name] = caption;
            }
            return caption;
        }

        public static string _ToCaption(this Type type)
        {
            StringBuilder sb = new StringBuilder();
            int ticLoc = type.Name.IndexOf('`');
            if (ticLoc >= 0)
                sb.Append(type.Name.Substring(0, ticLoc));
            else
                sb.Append(type.Name);
            
            if (type.GetGenericArguments().Any())
            {
                sb.Append("&lt;");
                sb.Append(string.Join(", ", type.GetGenericArguments().Select(g => g.ToCaption()).ToArray()));
                sb.Append("&gt;");
            }
            return sb.ToString();
        }

        public static string ToCref(this Type pt)
        {
            StringBuilder sb = new StringBuilder();
            string fullName = pt.FullName ?? pt.Name;
            if (pt.IsGenericParameter)
            {
                if (pt.DeclaringMethod != null)
                {
                    //generic parameter defined for this method
                    return string.Format("``{0}", pt.GenericParameterPosition);
                }
                else if (pt.DeclaringType != null)
                {
                    return pt.DeclaringType.GetGenericArguments().First(t => t.Name == pt.Name).Name;
                }
            }
            else
            {
                if (pt.IsGenericType && !pt.IsGenericTypeDefinition) //get rid of the tic if we have no open generic parameters
                {
                    int ticLoc = fullName.LastIndexOf("`");
                    if (ticLoc >= 0)
                    {
                        fullName = fullName.Substring(0, ticLoc);
                    }
                }
                sb.Append(fullName);

                if (pt.IsGenericType)
                {
                    sb.Append("{");
                    foreach (Type genericParameter in pt.GetGenericArguments())
                    {
                        sb.Append(ToCref(genericParameter));
                    }
                    sb.Append("}");
                }
            }
            return sb.ToString();
        }

        public static RxMemberInfoRef ToXMemberRef(this MemberInfo info)
        {
            RxMemberInfoRef memberRef = new RxMemberInfoRef();
            if (info != null)
            {
                Type t = info as Type;
                string fullName = t != null ? t.FullName : info.DeclaringType.FullName + "." + info.Name;

                memberRef.cref = string.Concat(info.MemberType.ToString()[0], ":", fullName);
                memberRef.caption = info.ToString().ToCaption();
            }
            return memberRef;
        }

        public static string ToXmlCommentID(this Type type)
        {
            string xid;
            if (type.DeclaringType != null) //Nested type
            {
                xid = string.Concat("T:", type.DeclaringType.Namespace, ".", type.DeclaringType.Name, ".", type.Name);
            }
            else //Standard type
            {
                xid = string.Concat("T:", type.Namespace, ".", type.Name);
            }
            return xid;
        }

        public static string ToXmlCommentID(this FieldInfo fieldInfo)
        {
            string xid = string.Concat("F:", fieldInfo.DeclaringType.ToXmlCommentID().Substring(2), ".", fieldInfo.Name);            
            return xid;
        }

        public static string ToXmlCommentID(this PropertyInfo propInfo)
        {
            string xid = string.Concat("F:", propInfo.DeclaringType.ToXmlCommentID().Substring(2), ".", propInfo.Name);
            return xid;
        }

        public static string ToXmlCommentID(this EventInfo eventInfo)
        {
            string xid = string.Concat("E:", eventInfo.DeclaringType.ToXmlCommentID().Substring(2), ".", eventInfo.Name);
            return xid;
        }


        public static string ToXmlCommentID(this MethodInfo methodInfo)
        {
            string xid = string.Concat("M:", methodInfo.DeclaringType.ToXmlCommentID().Substring(2), ".", methodInfo.Name);
            return xid;
        }

        public static string ToAnchor(this string caption)
        {
            return caption.Replace(' ', '_');
        }

        public static string ToAbsoluteUri(this string id)
        {
            return string.Concat(PagePrefix ?? "", id.Replace('.', '_').Replace('`', '_').Replace(':', '_'));
        }

        public static string[] CommaSplit(this string parameterString)
        {
            char[] parameters = parameterString.ToCharArray();

            //ensure we split only on top level commas for these sorts of cases A{B{C,D},E{F}}

            int pos = 0;
            int requiredCloseCount = 0;
            while (pos < parameters.Length - 1)
            {
                switch (parameters[pos])
                {
                    case '}':
                        requiredCloseCount--;
                        break;
                    case '{':
                        requiredCloseCount++;
                        break;
                    case ',':
                        if (requiredCloseCount == 0)
                            parameters[pos] = '|';
                        break;
                }
                pos++;
            }
            parameterString = new string(parameters);


            return parameterString.Split('|');
        }


        public static IEnumerable<Type> GetOpenGenericArguments(this MethodBase t)
        {
            if (t.IsGenericMethod)
            {
                if (t.DeclaringType != null)
                {
                    foreach (Type genericArgument in t.DeclaringType.GetOpenGenericArguments())
                    {
                        yield return genericArgument;
                    }
                }
            }

            foreach (Type genericArgument in t.GetGenericArguments())
            {
                yield return genericArgument;
            }
        }

        public static bool IsGenericParameterPlaceholder(this Type t)
        {
            return t.Name.StartsWith("__T");
        }

        public static int GetGenericParameterPlaceholderPosition(this Type t)
        {
            return int.Parse(t.Name.Substring(3));
        }

        public static IEnumerable<Type> GetOpenGenericArguments(this Type t)
        {
            if (t.IsGenericParameter)
            {
                if (t.DeclaringMethod != null)
                {
                    foreach (Type genericArgument in t.DeclaringMethod.GetOpenGenericArguments())
                    {
                        yield return genericArgument;
                    }
                }
                if (t.DeclaringType != null)
                {
                    foreach (Type genericArgument in t.DeclaringType.GetOpenGenericArguments())
                    {
                        yield return genericArgument;
                    }
                }
            }

            foreach (Type genericArgument in t.GetGenericArguments())
            {
                yield return genericArgument;
            }
        }
    }
}
