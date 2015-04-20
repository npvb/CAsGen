using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Semantic
{
    class SemanticContext
    {
        public IEnumerable<FunctionDeclaration> FunctionDeclarations {get; set;}
        public bool IdExistInSameScope(string id)
        {
            return false;
        }
        public bool IdExistInScope(string id)
        {
            return true;
        }

        public IRType GetIdType(string id) 
        {
            return new IntType();
        }

    }
}
