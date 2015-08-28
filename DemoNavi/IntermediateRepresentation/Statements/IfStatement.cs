using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class IfStatement : Statement
    {
     
        public Statement Statements { get; set; }
        public Expression Expressions { get; set; }
        public Statement IfFalse{ get; set; }

        public IfStatement(Expression expresion, Statement statement)
        {
            this.Expressions = expresion;
            this.Statements = statement;
        }

        public IfStatement(Expression expresion, Statement ifTrue, Statement ifFalse)
        {
            this.Expressions = expresion;
            this.Statements = ifTrue;
            this.IfFalse = ifFalse;
        }
        public override string ToString()
        {
            if (IfFalse == null) 
            {
                return "if ( " + Expressions + ")" + Statements;
            }
            return "if ( " + Expressions + ")" + Statements + "else" + IfFalse;
            
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
         //   throw new NotImplementedException();
        }

    }
}
