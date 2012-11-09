using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Refraxion
{
    /// <summary>
    /// A task that generates google code wiki pages for types with XmlDocumentation
    /// </summary>
    public partial class Compiler
    {

        public void Build(Compiler context, RxAssemblyInfo assembly, string nameSpace, IEnumerable<Type> types)
        {
            this.Assembly = assembly;
            id = string.Format("N:{0}", nameSpace);
            name = nameSpace;
            absoluteUri = id.ToAbsoluteUri();
            caption = name;
            BuildFileComments(Assembly);
            Assembly.Project.AddMember(this);

            List<RxTypeInfo> rxTypes = new List<RxTypeInfo>(types.Count());
            foreach (Type type in types)
            {
                rxTypes.Add(Assembly.FindOrBuildXType(context, type, type.ToXmlCommentID()));
            }
            this.type = rxTypes.OfType<RxTypeInfo>().ToArray();
        }
    }
}
