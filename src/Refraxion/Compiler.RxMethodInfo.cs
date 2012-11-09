using System.Reflection;
using System.Xml.Linq;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {
        public void Build(Compiler context, RxTypeInfo typeInfo, MethodInfo methodInfo, XElement methodMemberElement, string xid)
        {
            id = xid;
            caption = memberName = methodInfo.Name;
            SetUri(typeInfo, string.Concat("#", methodInfo.Name));
            BuildComments(context, methodMemberElement);
            isPublic = methodInfo.IsPublic;
            isStatic = methodInfo.IsStatic;

            memberInfo = methodInfo;
            //BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
        }
    }
}
