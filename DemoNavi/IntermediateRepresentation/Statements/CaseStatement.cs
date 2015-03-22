using DemoNavi.IntermediateRepresentation.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class CaseStatement : DefaultCaseStatement
    {
        public ValueExpression Value { get; set; }

        public CaseStatement(ValueExpression value, List<Statement> statementList) : base (statementList)
        {
            if (value == null) 
            {
                throw new ArgumentNullException("value");
            }
            this.Value = value;
        }
    }
}
