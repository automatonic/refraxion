using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Refraxion.Model
{
    public partial class RxTypeInfo
    {
        [XmlIgnore]
        internal Type Type;
        [XmlIgnore]
        internal Type ExplicitInterfaceType;
        [XmlIgnore]
        internal RxAssemblyInfo Parent;       
      

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
