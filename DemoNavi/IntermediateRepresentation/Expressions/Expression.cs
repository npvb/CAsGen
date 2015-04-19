using DemoNavi.IntermediateRepresentation.Semantic;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    abstract class Expression
    {
        public abstract IRType GetIRType();
        internal abstract void SemanticValidation(SemanticContext semanticContext);
    }
}
