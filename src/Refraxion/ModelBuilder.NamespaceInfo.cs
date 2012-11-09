using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Refraxion.Model;

namespace Refraxion
{
    public partial class ModelBuilder
    {

        RxNamespaceInfo BuildProjectNamespace(RxProjectInfo parent, string nameSpace, IEnumerable<Type> types)
        {
            RxNamespaceInfo info = new RxNamespaceInfo();
            //info.Assembly = parent;
            info.id = string.Format("N:{0}", nameSpace);
            info.name = nameSpace;
            info.absoluteUri = info.id.ToAbsoluteUri();
            info.caption = info.name;
            BuildFileComments(info);

            List<RxTypeInfo> rxTypes = new List<RxTypeInfo>(types.Count());
            foreach (Type type in types)
            {
                rxTypes.Add(FindOrBuildXType(type, type.ToXmlCommentID()));
            }
            info.type = rxTypes.OfType<RxTypeInfo>().ToArray();
            return info;
        }
    }
}
