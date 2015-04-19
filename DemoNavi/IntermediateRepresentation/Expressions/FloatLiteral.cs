using DemoNavi.IntermediateRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class FloatLiteral : ValueExpression
    {
        public FloatLiteral(double value)
        {
            base.Value = value;
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
