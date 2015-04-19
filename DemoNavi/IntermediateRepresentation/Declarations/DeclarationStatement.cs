using DemoNavi.IntermediateRepresentation.Semantic;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    public abstract class DeclarationStatement 
    {
        internal abstract void SemanticValidation(SemanticContext semanticContext);
    }
}
