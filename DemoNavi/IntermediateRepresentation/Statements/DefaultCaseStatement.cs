using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class DefaultCaseStatement
    {
        public List<Statement> StatementList { get; set; }

        public DefaultCaseStatement(List<Statement> statementList)
        {
            this.StatementList = statementList;
        }
    }
}
