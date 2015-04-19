﻿using DemoNavi.IntermediateRepresentation.Semantic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Statements
{
    class BlockStatement : Statement
    {
        public List<Statement> StatementList { get; set; }

        public BlockStatement(List<Statement> statementList)
        {
            if(statementList == null)
            {
                throw new NullReferenceException("statementList");
            }
            this.StatementList = statementList;                
        }

        internal override void SemanticValidation(SemanticContext semanticContext)
        {
            foreach (var statement in StatementList)
            {
                statement.SemanticValidation(semanticContext);
            }
        }
    }
}
