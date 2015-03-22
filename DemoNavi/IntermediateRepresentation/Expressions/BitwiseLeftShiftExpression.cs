using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class BitwiseLeftShiftExpression: BinaryExpression
    {
        private IntermediateRepresentation.Expression left;
        private IntermediateRepresentation.Expression right;

        public BitwiseLeftShiftExpression(IntermediateRepresentation.Expression left, IntermediateRepresentation.Expression right)
        {
            // TODO: Complete member initialization
            this.left = left;
            this.right = right;
        }
    }
}
