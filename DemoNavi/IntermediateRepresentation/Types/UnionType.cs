using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation.Types
{
    class UnionType : IRType
    {
        public string Id { get; set; }
        public List<DeclarationStatement> DeclarationStatement { get; set; }
        public UnionType(string id)
        {
            this.Id = id;
        }

        public UnionType(List<DeclarationStatement> declarationstatement)
        {
            this.DeclarationStatement = declarationstatement;
        }
    }
}
