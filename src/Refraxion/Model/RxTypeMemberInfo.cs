using System;
using System.Xml.Serialization;

namespace Refraxion.Model
{
    public partial class RxTypeMemberInfo
    {
        [XmlIgnore]
        internal Uri RelativeUri { get; private set; }
        [XmlIgnore]
        internal string MemberName { get; set; }
        [XmlIgnore]
        internal string ExplicitInterface { get; set; }
        [XmlIgnore]
        internal RxTypeInfo Parent { get; set; }

        public void SetUri(RxTypeInfo typeInfo, string relativeUri)
        {
            RelativeUri = new Uri(relativeUri);
            this.relativeUri = relativeUri;
            base.SetUri(typeInfo.AbsoluteUri, relativeUri);
        }
    }
}
