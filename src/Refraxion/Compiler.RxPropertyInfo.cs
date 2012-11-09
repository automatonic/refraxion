using System.Reflection;
using System.Xml.Linq;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {
        public static RxPropertyInfo Build(Compiler context, RxTypeInfo typeInfo, PropertyInfo propInfo, XElement fieldMemberElement, string xid)
        {
            id = xid;
            caption = memberName = propInfo.Name;
            BuildComments(context, fieldMemberElement);
            memberInfo = propInfo;
            propertyTypeRef = propInfo.PropertyType.ToXMemberRef();
            memberInfo = propInfo;
            canRead = propInfo.CanRead;
            canWrite = propInfo.CanWrite;
            //BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
        }
    }
}
