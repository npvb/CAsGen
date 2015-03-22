using DemoNavi.IntermediateRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Recompilers
{
    public abstract class Recompiler
    {
        public abstract string Recompile(Program program);
    }
}
