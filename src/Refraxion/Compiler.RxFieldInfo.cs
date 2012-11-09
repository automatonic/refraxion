using System.Reflection;
using System.Xml.Linq;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {
        public static RxFieldInfo Build(Compiler context, RxTypeInfo typeInfo, FieldInfo fieldInfo, XElement fieldMemberElement, string xid)
        {
            RxFieldInfo instance = new RxFieldInfo();

            instance.id = xid;
            instance.caption = instance.memberName = fieldInfo.Name;
            instance.SetUri(typeInfo, string.Concat("#", fieldInfo.Name));
            instance.BuildComments(context, fieldMemberElement);
            instance.isPublic = fieldInfo.IsPublic;
            instance.isStatic = fieldInfo.IsStatic;
            instance.isLiteral = fieldInfo.IsLiteral;
            if (fieldInfo.IsLiteral)
            {
                instance.literalValue = fieldInfo.GetRawConstantValue().ToString();
            }
            instance.memberInfo = fieldInfo;
            instance.fieldTypeRef = fieldInfo.FieldType.ToXMemberRef();
            //BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
            return instance;
        }
    }
}
