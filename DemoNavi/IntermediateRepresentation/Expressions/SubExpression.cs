using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class SubExpression : BinaryExpression
    {
        private IRType returnType;

        public SubExpression(Expression left, Expression right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override string ToString()
        {
            return base.Left.ToString() + " - " + base.Right.ToString();
        }

        public override Types.IRType GetIRType()
        {
            return returnType;
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            Left.SemanticValidation(semanticContext);
            Right.SemanticValidation(semanticContext);

            if (!(Left.GetIRType() is NumericType && Right.GetIRType() is NumericType))
            {
                throw new Semantic.SemanticValidationException("No se puede operar");
            }
            else
            {
                returnType = Left.GetIRType(); 
            }
        }
    }
}
