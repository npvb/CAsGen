using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Declarations
{
    class EnumValue
    {
        public string Id { get; set; }
        public int Value { get; set; }

        public EnumValue(string id)
        {
            this.Id = id;
        }
        public EnumValue(string id, int value)
        {
            this.Id = id;
            this.Value = value;
        }
    }
}
