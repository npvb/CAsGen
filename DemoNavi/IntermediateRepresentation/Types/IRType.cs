using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Types
{
    abstract class IRType
    {
        public static bool operator ==(IRType left, IRType right)
        {
            return object.ReferenceEquals(null,left) ? object.ReferenceEquals(null, right) : left.GetType() == right.GetType();
        }

        public static bool operator !=(IRType left, IRType right)
        {
            return !(left == right);
        }
    }
}
