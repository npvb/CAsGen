using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class PointerReferenceAccessExpr : Expression
    {
        public Expression Pointer { get; set; }
        public Expression Value { get; set; }

        public PointerReferenceAccessExpr(Expression pointer, Expression value)
        {
            this.Pointer = pointer;
            this.Value = value;
        }

        public override string ToString()
        {
            return Pointer.ToString() + "." + Value.ToString();
        }
    }
}
