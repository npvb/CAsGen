using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Declarations
{
    class EnumDeclaration
    {
        public string Id { get; set; }
        public List<EnumValue> EnumValue { get; set; }

        public EnumDeclaration(string id, List<EnumValue> enumValue)
        {
            this.Id = id;
            this.EnumValue = EnumValue;

        }
    }
}
