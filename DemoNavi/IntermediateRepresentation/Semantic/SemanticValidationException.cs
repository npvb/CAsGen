using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Semantic
{
    class SemanticValidationException : Exception
    {
        public SemanticValidationException(string message)
            : base(message)
        {
        }
    }
}
