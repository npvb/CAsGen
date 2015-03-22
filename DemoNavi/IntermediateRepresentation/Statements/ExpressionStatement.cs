using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class ExpressionStatement : Statement
    {
        public Expression Expression { get; set; }

        public ExpressionStatement(Expression expression)
        {
            this.Expression = expression;
        }
    }
}
