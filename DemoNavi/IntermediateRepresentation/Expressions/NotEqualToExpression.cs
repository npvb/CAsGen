using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class NotEqualToExpression : UnaryExpression
    {
        private IRType returnType;

        public NotEqualToExpression(Expression expr): base(expr)
        {

        }
        public override string ToString()
        {
            return "!" + base.Expression;
        }

        public override Types.IRType GetIRType()
        {
            return returnType;
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            Expression.SemanticValidation(semanticContext);
            if (!(Expression.GetIRType() is BoolType)) 
            {
                throw new Semantic.SemanticValidationException("No se puede negar");
            }else
                returnType = new BoolType(); //a exp tiene que poder negarse
        }
    }
}
