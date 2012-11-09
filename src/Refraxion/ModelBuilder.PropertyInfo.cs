using System.Reflection;
using System.Xml.Linq;
using Refraxion.Model;

namespace Refraxion
{
    public partial class ModelBuilder
    {
        RxPropertyInfo Build(RxTypeInfo parent, PropertyInfo propInfo, XElement element, string xid)
        {
            RxPropertyInfo info = new RxPropertyInfo();
            info.id = xid;
            info.caption = info.MemberName = propInfo.Name;
            info.SetUri(parent, string.Concat("#", propInfo.Name));
            BuildComments(info, element);
            info.MemberInfo = propInfo;
            info.propertyTypeRef = propInfo.PropertyType.ToXMemberRef();
            info.MemberInfo = propInfo;
            info.canRead = propInfo.CanRead;
            info.canWrite = propInfo.CanWrite;
            //BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
            return info;
        }
    }
}
