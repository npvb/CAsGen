using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoNavi.IntermediateRepresentation.Expressions;

namespace DemoNavi.Parser
{
    class EqualsExpression : BinaryExpression
    {
        private IntermediateRepresentation.Expression left;
        private IntermediateRepresentation.Expression right;

        public EqualsExpression(IntermediateRepresentation.Expression left, IntermediateRepresentation.Expression right)
        {
            // TODO: Complete member initialization
            this.left = left;
            this.right = right;
        }
        public override string ToString()
        {
            return base.Left.ToString() + " == " + base.Right.ToString();
        }
    }
}
