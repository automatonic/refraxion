using System.Reflection;
using System.Xml.Linq;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {
        public static RxEventInfo Build(Compiler context, RxTypeInfo rxTypeInfo, EventInfo eventInfo, XElement eventMemberElement, string xid)
        {
            RxEventInfo instance = new RxEventInfo();
            instance.id = xid;
            instance.caption = instance.memberName = eventInfo.Name;
            instance.SetUri(rxTypeInfo, string.Concat("#", eventInfo.Name));
            instance.BuildComments(context, eventMemberElement);
            instance.memberInfo = eventInfo;
            //instance.BuildAttributesElement(memberElement, memberInfo.GetCustomAttributes(false));
            return instance;
        }
    }
}
