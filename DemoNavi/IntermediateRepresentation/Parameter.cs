using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    using DemoNavi.IntermediateRepresentation.Types;
    class Parameter
    {
         private IRType type;
        private string id;

        internal IRType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public Parameter(IRType type, string id)
        {
            // TODO: Complete member initialization
            this.type = type;
            this.id = id;
        }
        public override string ToString()
        {
            return type.ToString() + " " + id;
        }
    }
}
