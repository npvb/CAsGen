using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    abstract class ValueExpression : Expression
    {
        public object Value { get; set; }
    }
}
