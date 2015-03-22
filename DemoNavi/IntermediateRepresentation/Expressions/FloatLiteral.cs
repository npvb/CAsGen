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
        
    }
}
