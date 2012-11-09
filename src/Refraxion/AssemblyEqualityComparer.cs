using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Refraxion
{
    public class AssemblyEqualityComparer : IEqualityComparer<Assembly>
    {
        public bool Equals(Assembly x, Assembly y)
        {
            return x.GetName().Equals(y.GetName());
        }

        public int GetHashCode(Assembly obj)
        {
            return obj.GetName().GetHashCode();
        }
    }
}
