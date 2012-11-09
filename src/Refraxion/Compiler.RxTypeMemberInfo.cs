using System;
using System.Xml.Serialization;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {

        public void SetUri(RxTypeInfo typeInfo, string relativeUri)
        {
            RelativeUri = new Uri(relativeUri);
            this.relativeUri = relativeUri;
            base.SetUri(typeInfo.AbsoluteUri, relativeUri);
        }
    }
}
