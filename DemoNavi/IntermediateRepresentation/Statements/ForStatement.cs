using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class ForStatement : Statement
    {
        public Expression Init { get; set; }
        public Expression Condition { get; set; }
        public Expression Loop { get; set; }
        public Statement Body { get; set; }
        public ForStatement(Expression init, Expression condition, Expression loop, Statement body)
        {
            this.Init = init;
            this.Condition = condition;
            this.Loop = loop;
            this.Body = body;
        }

        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            throw new NotImplementedException();
        }
    }
}
