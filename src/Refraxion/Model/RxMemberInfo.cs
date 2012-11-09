using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Refraxion.Model
{
    public abstract partial class RxMemberInfo
    {
        [XmlIgnore]
        internal Uri AbsoluteUri { get; private set; }

        [XmlIgnore]
        internal MemberInfo MemberInfo { get; set; }

        [XmlIgnore]
        internal bool IsPublic { get; set; }

        public override string ToString()
        {
            return String.Concat( id, " ", AbsoluteUri);
        }

        public class EqualityComparer : IEqualityComparer<RxMemberInfo>
        {
            public bool Equals(RxMemberInfo x, RxMemberInfo y)
            {
                return x.id.Equals(y.id);
            }

            public int GetHashCode(RxMemberInfo obj)
            {
                return obj.id.GetHashCode();
            }
        }

        public void SetUri(string uri)
        {
            AbsoluteUri = new Uri(uri);
            absoluteUri = AbsoluteUri.ToString();
        }

        protected void SetUri(Uri baseUri, string relativeUri)
        {
            AbsoluteUri = new Uri(baseUri, relativeUri);
            absoluteUri = AbsoluteUri.ToString();
        }
    }

    public class RxMemberLiteral : RxMemberInfo
    {
    }

}
