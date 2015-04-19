using DemoNavi.IntermediateRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class NotEqualExpression : BinaryExpression
    {
        private IntermediateRepresentation.Expression left;
        private IntermediateRepresentation.Expression right;

        public NotEqualExpression(IntermediateRepresentation.Expression left, IntermediateRepresentation.Expression right)
        {
            // TODO: Complete member initialization
            this.left = left;
            this.right = right;
        }

        public override string ToString()
        {
            return base.Left.ToString() + " != " + base.Right.ToString();
        }

        public override Types.IRType GetIRType()
        {
            throw new NotImplementedException();
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            throw new NotImplementedException();
        }
    }
}
