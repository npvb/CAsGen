using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Types
{
    class StructType : IRType
    {
        public string Id { get; set; }
        public List<DeclarationStatement> DeclarationStatement { get; set; }
        public StructType(string id)
        {
            this.Id = id;
        }

        public StructType( List<DeclarationStatement> declarationstatement)
        {
            this.DeclarationStatement = declarationstatement;
        }
    }
}
