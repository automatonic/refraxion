using System;
using System.Xml.Serialization;

namespace Refraxion.Model
{
    public partial class RxTypeMemberInfo
    {
        [XmlIgnore]
        public Uri RelativeUri { get; private set; }
        [XmlIgnore]
        public string memberName;
        [XmlIgnore]
        public string explicitInterface;

        public void SetUri(RxTypeInfo typeInfo, string relativeUri)
        {
            RelativeUri = new Uri(relativeUri);
            this.relativeUri = relativeUri;
            base.SetUri(typeInfo.AbsoluteUri, relativeUri);
        }
    }
}
