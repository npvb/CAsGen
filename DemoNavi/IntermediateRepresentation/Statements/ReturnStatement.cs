using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    class ReturnStatement : Statement
    {
        public Expression ReturnExpression { get; set; }
        public ReturnStatement(Expression value)
        {
            this.ReturnExpression = value;
        }

        public ReturnStatement()
        {

        }

        public override string ToString()
        {
            return "return " + ReturnExpression.ToString();
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            throw new NotImplementedException();
        }
    }
}
