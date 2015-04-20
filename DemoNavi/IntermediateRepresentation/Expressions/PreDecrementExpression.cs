using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class PreDecrementExpression : UnaryExpression
    {
        private IRType returnType;

        public PreDecrementExpression(Expression expr): base(expr)
        {

        }

        public override string ToString()
        {
            return "--" + base.Expression;
        }

        public override Types.IRType GetIRType()
        {
            return returnType;
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            Expression.SemanticValidation(semanticContext);
            if (!(Expression.GetIRType() is NumericType))
            {
                throw new Semantic.SemanticValidationException("No se puede PreDecrement");
            }
            else
                returnType = new IntType();
        }
    }
}
