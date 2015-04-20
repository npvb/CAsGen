using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoNavi.IntermediateRepresentation.Expressions;
using DemoNavi.IntermediateRepresentation.Types;

namespace DemoNavi.IntermediateRepresentation.Expressions
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
            return new BoolType();
        }

        internal override void SemanticValidation(IntermediateRepresentation.Semantic.SemanticContext semanticContext)
        {
            Left.SemanticValidation(semanticContext);
            Right.SemanticValidation(semanticContext);

            if (!(Left.GetIRType() is NumericType && Right.GetIRType() is NumericType) || !(Left.GetIRType() == Right.GetIRType()))
            {
                throw new Semantic.SemanticValidationException("No se puede asignar");

            }

        }
    }
}
