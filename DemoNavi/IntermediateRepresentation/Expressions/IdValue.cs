using DemoNavi.IntermediateRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.Parser
{
    class IdValue : Expression
    {
        private object p;

        public IdValue(object p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }

        public override string ToString()
        {
            return p.ToString();
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
