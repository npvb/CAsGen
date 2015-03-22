using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class NotEqualToExpression : BinaryExpression
    {
        private IntermediateRepresentation.Expression left;
        private IntermediateRepresentation.Expression right;

        public NotEqualToExpression(IntermediateRepresentation.Expression left, IntermediateRepresentation.Expression right)
        {
            // TODO: Complete member initialization
            this.left = left;
            this.right = right;
        }

        public override string ToString()
        {
            return base.Left.ToString() + " '!=' " + base.Right.ToString();
        }
    }
}
