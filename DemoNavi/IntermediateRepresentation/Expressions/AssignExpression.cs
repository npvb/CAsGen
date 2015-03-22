using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class AssignExpression : BinaryExpression
    {

        public AssignExpression(Expression left, Expression right)
        {
            // TODO: Complete member initialization
            base.Left = left;
            base.Right = right;
        }

        public override string ToString()
        {
            return base.Left.ToString() + " = " + base.Right.ToString();
        }
    }
}
