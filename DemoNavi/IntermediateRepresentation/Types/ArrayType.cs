using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Types
{
    class ArrayType : IRType
    {
        public IRType Type { get; set; }
        public long Size { get; set; }
        public ArrayType(long size)
        {
            this.Size = size;
        }

        public ArrayType()
        {

        }
    }
}
