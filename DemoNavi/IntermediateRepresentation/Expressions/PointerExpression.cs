using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class PointerExpression: UnaryExpression
    {
        public PointerExpression(Expression expr) : base(expr)
        {

        }
        public override string ToString()
        {
            return " '*' ";
        }
    }
}
