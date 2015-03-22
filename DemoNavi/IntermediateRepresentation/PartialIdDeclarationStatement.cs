using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    class PartialIdDeclarationStatement
    {
        public ArrayType ArrayType { get; set; }
        public IdDeclarationStatement DeclarationStatement { get; set; }

        public PartialIdDeclarationStatement(IdDeclarationStatement declarationStatement, ArrayType arrayType)
        {
            this.ArrayType = arrayType;
            this.DeclarationStatement = declarationStatement;
        }
    }
}
