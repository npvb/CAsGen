using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    using DemoNavi.IntermediateRepresentation.Semantic;
    using DemoNavi.IntermediateRepresentation.Statements;
    using DemoNavi.IntermediateRepresentation.Types;
    class FunctionDeclaration : DeclarationStatement
    {
        private IRType type;
        private string id;
        private BlockStatement block;

        internal Types.IRType Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public List<Parameter> Parameters { get; set; }

        public FunctionDeclaration(IRType type, string id)
        {
            this.type = type;
            this.id = id;
            this.Parameters = new List<Parameter>();
        }

        public BlockStatement Block
        {
            get
            {
                return this.block;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("value");
                }
                this.block = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2}) {{ {3} }}", type, id, string.Join(",", Parameters), string.Join(Environment.NewLine, Block.StatementList));
        }

        internal override void SemanticValidation(SemanticContext semanticContext)
        {
            /*bool multiMatch =  semanticContext.FunctionDeclarations.Where(f =>
            {
                bool match = true;
                match = f.Id == this.Id && f.Parameters.Count == this.Parameters.Count;
                if (match)
                {
                    for (int i = 0; i < f.Parameters.Count; i++)
                    {
                        match = f.Parameters[i].Type == this.Parameters[i].Type;
                        if (!match)
                            break;
                    }
                }
                return match;
            }).Count() == 1;

            if (!multiMatch) 
            {
                throw new SemanticValidationException("Ambiguous Type of Function Definition: " + id);
            }
            Block.SemanticValidation(semanticContext);*/
        }
    }
}
