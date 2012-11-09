using System.Reflection;
using System.Xml.Linq;
using Refraxion.Model;

namespace Refraxion
{
    public partial class ModelBuilder
    {
        RxMethodInfo BuildTypeMethod(RxTypeInfo parent, MethodInfo methodInfo, XElement element, string xid)
        {
            RxMethodInfo info = new RxMethodInfo();
            info.id = info.id;
            info.caption = info.name = methodInfo.Name;
            info.SetUri(parent, string.Concat("#", methodInfo.Name));
            BuildComments(info, element);
            info.IsPublic = methodInfo.IsPublic;
            info.isStatic = methodInfo.IsStatic;

            info.MemberInfo = methodInfo;
            //BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
            return info;
        }
    }
}
