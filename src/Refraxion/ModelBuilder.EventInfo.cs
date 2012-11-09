using System.Reflection;
using System.Xml.Linq;
using Refraxion.Model;

namespace Refraxion
{
    public partial class ModelBuilder
    {
        RxEventInfo BuildTypeEvent(RxTypeInfo parent, EventInfo eventInfo, XElement element, string xid)
        {
            RxEventInfo info = new RxEventInfo();
            info.id = xid;
            info.caption = info.MemberName = eventInfo.Name;
            info.SetUri(parent, string.Concat("#", eventInfo.Name));
            BuildComments(info, element);
            info.MemberInfo = eventInfo;
            //instance.BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
            return info;
        }
    }
}
