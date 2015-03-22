using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class PreDecrementExpression : UnaryExpression
    {
        public PreDecrementExpression(Expression expr): base(expr)
        {

        }
        public override string ToString()
        {
            return " '--' ";
        }
    }
}
