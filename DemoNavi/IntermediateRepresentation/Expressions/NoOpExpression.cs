using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class NoOpExpression : Expression
    {
        public override Types.IRType GetIRType()
        {
            return new VoidType();
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
           
        }
    }
}
