using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

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
        [XmlIgnore]
        public Type type;
        [XmlIgnore]
        public Type explicitInterfaceType;
        [XmlIgnore]
        public RxAssemblyInfo Assembly;
        [XmlIgnore]
        public List<RxTypeMemberInfo> members = new List<RxTypeMemberInfo>();

        /// <summary>
        /// Builds the type.
        /// </summary>
        /// <param name="typeElement">The type element.</param>
        /// <param name="xid">The id.</param>
        /// <returns></returns>
        public static RxTypeInfo Build(Compiler context, RxAssemblyInfo assembly, Type type, XElement typeElement, string xid)
        {
            RxTypeInfo instance = new RxTypeInfo();
            instance.name = type.Name;
            instance.Assembly = assembly;
            //Fill common parameters
            instance.id = xid;
            instance.caption = type.Name;
            instance.@namespace = type.Namespace;
            instance.type = type;
            instance.isPublic = type.IsPublic;

            if (typeElement != null)
                BuildComments(context, typeElement);

            instance.isNested = type.IsNested;
            instance.isSealed = type.IsSealed;
            instance.isAbstract = type.IsAbstract;
            var fields = BuildXFields(context, instance.Assembly, type, typeElement).ToList();
            fields.ForEach(field => instance.Assembly.Project.AddMember(field));
            instance.field = fields.ToArray();
            instance.property = instance.BuildProperties(context, type, typeElement).ToArray();
            instance.method = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(propertyInfo => 
                {
                    string xcid = propertyInfo.ToXmlCommentID();
                    XElement commentElement = assembly.GetCommentElement(context, xcid);
                    RxPropertyInfo info = RxPropertyInfo.Build(context, this, propertyInfo, commentElement, xcid);
                    assembly.Project.AddMember(info);
                    return info;
                })
                
                instance.BuildMethods(context, type, typeElement).ToArray();
            instance.@event = instance.BuildEvents(context, type, typeElement).ToArray();

            if (type.IsNested)
            {
                instance.caption = string.Concat(type.DeclaringType.Name, ".", instance.caption);
            }
        }


        public static IEnumerable<RxFieldInfo> BuildXFields(Compiler context, RxAssemblyInfo assembly, Type type, XElement typeElement)
        {
            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (!fieldInfo.IsPublic)
                    continue;
                string xid = fieldInfo.ToXmlCommentID();
                XElement commentElement = assembly.GetCommentElement(context, xid);
                RxFieldInfo info = RxFieldInfo.Build(context, this, fieldInfo, commentElement, xid);
                yield return info;
            }
        }

        public static IEnumerable<RxMethodInfo> BuildMethods(Compiler context, Type type, XElement typeElement)
        {
            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (!methodInfo.IsPublic)
                    continue;
                string xid = methodInfo.ToXmlCommentID();
                XElement commentElement = Assembly.GetCommentElement(context, xid);
                RxMethodInfo info = RxMethodInfo.Build(context, this, methodInfo, commentElement, xid);
                Assembly.Project.AddMember(info);
                yield return info;
            }
        }

        public IEnumerable<RxEventInfo> BuildEvents(Compiler context, Type type, XElement typeElement)
        {
            foreach (EventInfo eventInfo in type.GetEvents(BindingFlags.Public | BindingFlags.Static))
            {
                string xid = eventInfo.ToXmlCommentID();
                XElement commentElement = Assembly.GetCommentElement(context, xid);
                RxEventInfo info = RxEventInfo.Build(context, this, eventInfo, commentElement, xid);
                Assembly.Project.AddMember(info);
                yield return info;
            }
        }

        public IEnumerable<RxPropertyInfo> BuildProperties(Compiler context, Type type, XElement typeElement)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                
                yield return info;
            }
        }




        //public static void ParseMethodParameters(string parameters, out List<Type> paramTypes, out ParameterModifier parameterModifier)
        //{
        //    paramTypes = new List<Type>();

        //    string[] paramParts = parameters.CommaSplit();

        //    //In case we encounter a pass-by-ref parameter
        //    parameterModifier = new ParameterModifier(paramParts.Length);

        //    int iParamIndex = 0;
        //    //Process each parameter individually
        //    foreach (string paramPart in paramParts)
        //    {
        //        bool isRef = false;
        //        string safeParamPart = paramPart;
        //        if (paramPart.EndsWith("@"))
        //        {
        //            safeParamPart = paramPart.Remove(paramPart.Length - 1);
        //            isRef = true;
        //        }

        //        Type paramType = null;
        //        if (safeParamPart.StartsWith("``"))
        //        {
        //            int pos = int.Parse(safeParamPart.Substring(2));
        //            paramType = typeof(GenericArgumentPlaceholderHost<,,,,,,,,,,,,,>).GetGenericArguments()[pos];
        //        }
        //        else if (safeParamPart.StartsWith("`") && xType.type != null && xType.type.IsGenericTypeDefinition)
        //        {
        //            //a generic parameter defined in the generic type definition
        //            paramType = ParseGenericTypeMethodParameter(xType.type, safeParamPart);
        //        }
        //        else
        //        {
        //            //a normal parameter, just load the related type
        //            paramType = xType.assembly.project.GetType(safeParamPart);
        //        }

        //        if (paramType != null)
        //        {
        //            paramTypes.Add(paramType);
        //            parameterModifier[iParamIndex] = isRef;
        //        }
        //        else
        //        {
        //            xType.assembly.LogWarning("Failed to resolve parameter type for {0}", safeParamPart);
        //            //Bail                    
        //            paramTypes = null;
        //            break;
        //        }
        //        iParamIndex++;
        //    }
        //}

        private static Type ParseGenericTypeMethodParameter(Type parentType, string typeName)
        {
            string genericName = typeName;
            bool buildArray = typeName.EndsWith("[]");
            if (buildArray) //Parse array syntax
            {
                genericName = typeName.Remove(typeName.Length - 2);
            }

            int paramIndex = int.Parse(genericName.Substring(1));
            Type typeArg = parentType.GetGenericArguments()[paramIndex];

            if (buildArray)
            {
                typeArg = typeArg.MakeArrayType();
            }

            return typeArg;
        }



        public static string MangleGenericTypeNames(string typeName)
        {
            int firstCurly = typeName.IndexOf("{");
            if (firstCurly >= 0)
            {
                string prefix = typeName.Substring(0, firstCurly);
                string suffix = typeName.Substring(firstCurly + 1);
                string array = string.Empty;
                if (typeName.EndsWith("]"))
                {
                    int brackPos = suffix.IndexOf('[');
                    array = suffix.Substring(brackPos, suffix.Length - brackPos);
                    suffix = suffix.Remove(brackPos - 1);
                }
                else
                {
                    suffix = suffix.Remove(suffix.Length - 1);
                }

                string[] parts = suffix.CommaSplit();
                string typeCount = string.Format("`{0}", parts.Length);
                List<string> mangledTypeNames = new List<string>(parts.Length);
                foreach (string part in parts)
                {
                    mangledTypeNames.Add(MangleGenericTypeNames(part));
                }
                return string.Concat(prefix, typeCount, "[", string.Join(",", mangledTypeNames), "]", array);
            }
            else
                return typeName;
        }
    }

    public class GenericArgumentPlaceholderHost<__T0, __T1, __T2, __T3, __T4, __T5, __T6, __T7, __T8, __T9, __T10, __T11, __T12, __T13>
    {
    }

    public static class XTypeExtensions
    {


    }

}
