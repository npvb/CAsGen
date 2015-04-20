using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.Parser
{
    class IdValue : Expression
    {
        private string id;
        private IRType returnType;
        public IdValue(string id)
        {
            // TODO: Complete member initialization
            this.id = id;
        }

        public override string ToString()
        {
            return id;
        }

        public override IntermediateRepresentation.Types.IRType GetIRType()
        {
            return returnType;
        }

        internal override void SemanticValidation(IntermediateRepresentation.Semantic.SemanticContext semanticContext)
        {
            semanticContext.IdExistInScope(this.id);
            returnType = semanticContext.GetIdType(this.id);
        }
    }
}
