using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoNavi.IntermediateRepresentation.Expressions;

namespace DemoNavi.Parser
{
    class EqualsExpression : BinaryExpression
    {
        public EqualsExpression(IntermediateRepresentation.Expression left, IntermediateRepresentation.Expression right)
        {
            // TODO: Complete member initialization
            this.Left = left;
            this.Right = right;
        }
        public override string ToString()
        {
            return base.Left.ToString() + " == " + base.Right.ToString();
        }

        public override IntermediateRepresentation.Types.IRType GetIRType()
        {
            throw new NotImplementedException();
        }



        internal override void SemanticValidation(IntermediateRepresentation.Semantic.SemanticContext semanticContext)
        {
            throw new NotImplementedException();
        }
    }
}
