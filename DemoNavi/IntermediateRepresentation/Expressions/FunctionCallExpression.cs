using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class FunctionCallExpression : Expression
    {
        public ExpressionList Parameters { get; set; }
        public string Id { get; set; }

        public FunctionCallExpression(string id, Expression expr) : this(id)
        {
            this.Id = id;
            if (expr is ExpressionList)
            {
                this.Parameters = expr as ExpressionList;
            }
            else
            {
                this.Parameters = new ExpressionList(expr);
            }
            
        }

        public FunctionCallExpression(string id) 
        {
            this.Id = id;
            this.Parameters = new ExpressionList();
        }

        public override string ToString()
        {
            return Id + "(" + Parameters + ")";
        }
    }
}
