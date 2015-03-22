using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    public class Program
    {
        public List<DeclarationStatement> Declarations { get; set; }
        public Program(List<DeclarationStatement> declarations)
        {
            this.Declarations = declarations;
        }
    }
}
