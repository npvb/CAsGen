using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.Parser
{
    class CharLiteral : ValueExpression
    {
        public CharLiteral(byte value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
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
