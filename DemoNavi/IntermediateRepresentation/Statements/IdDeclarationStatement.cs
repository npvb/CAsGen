using DemoNavi.IntermediateRepresentation.Modifiers;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    class IdDeclarationStatement : Statement
    {
        public string Id { get; set; }
        public Expression InitializationExpression { get; set; }
        public IRType Type { get; set; }
        public Modifier Modifier { get; set; }
        public IdDeclarationStatement(string id, Expression initializationExpression) : this(id)
        {
            this.InitializationExpression = initializationExpression;
        }

        public IdDeclarationStatement(string id)
        {
            this.Id = id;
            this.Modifier = new AutoModifier();
        }
        public IdDeclarationStatement(string id, Expression initializationExpression, IRType type)
        {
            this.Id = id;
            this.InitializationExpression = initializationExpression;
            this.Type = type;
        }
    }
}
