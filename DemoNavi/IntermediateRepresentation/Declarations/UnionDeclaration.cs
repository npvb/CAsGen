﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Declarations
{
    class UnionDeclaration : DeclarationStatement
    {
        public string Id { get; set; }
        public List<IdDeclarationStatement> DeclarationStatement { get; set; }

        public UnionDeclaration(string id, List<IdDeclarationStatement> declarationstatement)
        {
            this.Id = id;
            this.DeclarationStatement = declarationstatement;
        }
        internal override void SemanticValidation(Semantic.SemanticContext semanticContext)
        {
            throw new NotImplementedException();
        }
    }
}
