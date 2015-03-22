using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Types
{
    class PointerType : IRType
    {
        private IRType baseType;

        internal IRType BaseType
        {
            get { return baseType; }
            set { baseType = value; }
        }

        public PointerType(IRType baseType)
        {
            this.baseType = baseType;
        }
        public override string ToString()
        {
            return baseType.ToString() + "*";
        }
    }
}
