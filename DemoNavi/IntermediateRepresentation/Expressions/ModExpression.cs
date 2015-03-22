using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class ModExpression : BinaryExpression
     {
        public ModExpression(IntermediateRepresentation.Expression left, IntermediateRepresentation.Expression right)
        {
            // TODO: Complete member initialization
            this.Left = left;
            this.Right = right;
        }
        public override string ToString()
        {
            return base.Left.ToString() + " &= " + base.Right.ToString();
        }
    }
}
