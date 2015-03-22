using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    class VarItem
    {
        public int Pointers {get; set;}
        public PartialIdDeclarationStatement PartialIdDeclaration { get; set; }

        public VarItem(int pointers, PartialIdDeclarationStatement partialIdDeclaration)
        {
            // TODO: Complete member initialization
            this.Pointers = pointers;
            this.PartialIdDeclaration = partialIdDeclaration;
        }
    }
}
