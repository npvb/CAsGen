using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class ExpressionList : Expression
    {
        public List<Expression> Exprlist { get; set; }

        internal ExpressionList()
        {
            this.Exprlist = new List<Expression>();
        }

        internal ExpressionList(Expression head) : this()
        {
            this.Exprlist.Add(head);
        }
        public ExpressionList(ExpressionList expressionList, Expression expression) : this()
        {
            this.Exprlist.AddRange(expressionList.Exprlist);
            this.Exprlist.Add(expression);
        }
        public ExpressionList(Expression head, Expression tail) : this()
        {
            this.Exprlist.Add(head);
            this.Exprlist.Add(tail);
        }

        public override string ToString()
        {
            return string.Join(",", Exprlist.Select(p => p.ToString()));
        }

        public override Types.IRType GetIRType()
        {
            throw new NotImplementedException();
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            throw new NotImplementedException();
        }
    }
}
