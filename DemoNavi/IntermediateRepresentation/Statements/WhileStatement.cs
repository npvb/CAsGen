using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class WhileStatement : Statement
    {
     
        public Statement Statements { get; set; }
        public Expression Expressions { get; set; }
        public WhileStatement(Expression expresion, Statement statement)
        {
            this.Expressions = expresion;
            this.Statements = statement;
        }

        public override string ToString()
        {
            return "while ( " + Expressions + ")" + Statements;
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            //throw new NotImplementedException();
        }
    }
}
