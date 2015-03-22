﻿using DemoNavi.IntermediateRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class SubExpression : BinaryExpression
    {
        public SubExpression(Expression left, Expression right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override string ToString()
        {
            return base.Left.ToString() + " - " + base.Right.ToString();
        }
    }
}
