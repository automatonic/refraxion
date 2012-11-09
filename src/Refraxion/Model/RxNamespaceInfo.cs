using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Refraxion.Model
{
    public partial class RxNamespaceInfo
    {
        [XmlIgnore]
        public RxAssemblyInfo Assembly { get; private set; }
    }
}
