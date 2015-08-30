using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class SwitchStatement : Statement
    {
        public List<DefaultCaseStatement> CaseStatements { get; set; }
        public Expression Expressions { get; set; }
        public SwitchStatement(Expression expresion, List<DefaultCaseStatement> caseStatements)
        {
            this.Expressions = expresion;
            this.CaseStatements = caseStatements;
            
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
           // throw new NotImplementedException();
        }
    }
}
