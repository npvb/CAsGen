using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class DoStatement : Statement
    {
        public Statement Statements { get; set; }
        public Expression Expressions { get; set; }
        public DoStatement(Statement statement,Expression expresion)
        {
            this.Expressions = expresion;
            this.Statements = statement;
        }

        public override string ToString()
        {
            return "do {" + Statements + "} while ( " + Expressions + ")" ;
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            throw new NotImplementedException();
        }
    }
}
