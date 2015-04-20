using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class FloatLiteral : ValueExpression
    {
        private IRType returnType;
        public FloatLiteral(double value)
        {
            base.Value = value;
        }


        public override Types.IRType GetIRType()
        {
            return returnType;
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            returnType = new FloatType();
        }
    }
}
