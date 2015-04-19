using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Expressions
{
    class FunctionCallExpression : Expression
    {
        public FunctionDeclaration FunctionDeclaration { get; set; }
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

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            this.FunctionDeclaration = semanticContext.FunctionDeclarations.Where(f => {
                bool match = true;
                match = f.Id == this.Id && f.Parameters.Count == this.Parameters.Exprlist.Count;
                if ( match )
                {
                    for (int i = 0; i < f.Parameters.Count; i++)
                    {
                        match = f.Parameters[i].Type == this.Parameters.Exprlist[i].GetIRType();
                        if (!match)
                            break;
                    }
                }
                return match;
            }).First();

        }

        public override Types.IRType GetIRType()
        {
            throw new NotImplementedException();
        }
    }
}
