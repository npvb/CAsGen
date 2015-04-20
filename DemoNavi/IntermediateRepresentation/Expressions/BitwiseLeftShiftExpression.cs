using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class BitwiseLeftShiftExpression: BinaryExpression
    {
        private IntermediateRepresentation.Expression left;
        private IntermediateRepresentation.Expression right;
        private IRType returnType;

        public BitwiseLeftShiftExpression(IntermediateRepresentation.Expression left, IntermediateRepresentation.Expression right)
        {
            // TODO: Complete member initialization
            this.left = left;
            this.right = right;
        }

        public override Types.IRType GetIRType()
        {
            return returnType;
        }

        public override string ToString()
        {
            return base.Left.ToString() + "<< " + base.Right.ToString();
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            Left.SemanticValidation(semanticContext);
            Right.SemanticValidation(semanticContext);

            if (!(Left.GetIRType() is NumericType && Right.GetIRType() is NumericType))
            {
                throw new Semantic.SemanticValidationException("No se puede asignar");
            }
            else
            {
                returnType = Left.GetIRType(); //evaluar cual tipo de podría asignar dependiendo de su tamanio
            }
        }
    }
}
