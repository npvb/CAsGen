﻿using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Expressions;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoNavi.Parser
{
    class StringLiteral : ValueExpression
    {

        public StringLiteral(object value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override IntermediateRepresentation.Types.IRType GetIRType()
        {
            //TODO:
            return new VoidType();
        }

        internal override void SemanticValidation(IntermediateRepresentation.Semantic.SemanticContext semanticContext)
        {
           //TODO:
        }
    }
}
