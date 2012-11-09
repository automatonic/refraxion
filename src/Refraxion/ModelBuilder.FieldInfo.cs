using System.Reflection;
using System.Xml.Linq;
using Refraxion.Model;

namespace Refraxion
{
    public partial class ModelBuilder
    {
        RxFieldInfo BuildTypeField(RxTypeInfo parent, FieldInfo fieldInfo, XElement element, string xid)
        {
            RxFieldInfo info = new RxFieldInfo();

            info.id = xid;
            info.caption = info.MemberName = fieldInfo.Name;
            info.SetUri(parent, string.Concat("#", fieldInfo.Name));
            BuildComments(info, element);
            info.IsPublic= fieldInfo.IsPublic;
            info.isStatic= fieldInfo.IsStatic;
            info.isLiteral = fieldInfo.IsLiteral;
            if (fieldInfo.IsLiteral)
            {
                info.literalValue = fieldInfo.GetRawConstantValue().ToString();
            }
            info.MemberInfo = fieldInfo;
            info.fieldTypeRef = fieldInfo.FieldType.ToXMemberRef();
            //BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
            return info;
        }
    }
}
