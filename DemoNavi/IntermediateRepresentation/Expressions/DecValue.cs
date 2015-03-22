using DemoNavi.IntermediateRepresentation.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    class DecValue : ValueExpression
    {
        public DecValue(object value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

    }
}
