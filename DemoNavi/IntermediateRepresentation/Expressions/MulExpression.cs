using DemoNavi.IntermediateRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class MulExpression : BinaryExpression
    {

        public MulExpression(Expression left, Expression right)
        {
            base.Left = left;
            base.Right = right;
        }

        public override string ToString()
        {
            return base.Left.ToString() + " * " + base.Right.ToString();
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
