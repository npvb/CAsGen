using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    abstract class UnaryExpression : Expression
    {
        public Expression Expression { get; set; }

        public UnaryExpression(Expression expr)
        {
            this.Expression = expr;
        }
    }
}
