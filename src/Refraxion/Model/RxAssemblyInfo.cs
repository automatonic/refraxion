using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Refraxion.Model
{
    public partial class RxAssemblyInfo
    {
        [XmlIgnore]
        internal RxProjectInfo Parent { get; set; }

        [XmlIgnore]
        internal Assembly Assembly { get; set; }

        [XmlIgnore]
        internal Dictionary<string, MethodInfo> ExtensionMethodLookup { get; set; }

        [XmlIgnore]
        internal IDictionary<string, XElement> MemberXmlLookup { get; set; }

        [XmlIgnore]
        internal List<string> Namespaces { get; set; }

    }


}
