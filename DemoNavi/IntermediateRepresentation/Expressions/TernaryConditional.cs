using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class TernaryConditional
    {
        public Expression Condition { get; set; }
        public Expression IfTrue { get; set; }
        public Expression IfFalse { get; set; }

        public TernaryConditional(Expression condition, Expression ifTrue, Expression ifFalse )
        {
            this.Condition = condition;
            this.IfTrue = ifTrue;
            this.IfFalse = ifFalse;
        }
    }
}
