using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class OneComplementExpression: UnaryExpression
    {
        public OneComplementExpression(Expression expr): base(expr)
        {

        }
        public override string ToString()
        {
            return " '~' ";
        }
    }
}
