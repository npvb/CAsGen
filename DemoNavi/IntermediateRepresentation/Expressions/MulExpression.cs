using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class MulExpression : BinaryExpression
    {
        private IRType returnType;

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
                returnType = Left.GetIRType(); //evaluar cual tipo de podría asignar dependiendo de su tamanio
            }
        }
    }
}
