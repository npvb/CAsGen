using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Types
{
    class EnumType: IRType
    {
        public string Id { get; set; }
        public EnumType(string id)
        {
            this.Id = id;
        }

    }
}
