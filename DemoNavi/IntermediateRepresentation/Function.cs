using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    using DemoNavi.IntermediateRepresentation.Statements;
    using DemoNavi.IntermediateRepresentation.Types;
    class Function : DeclarationStatement
    {
        private IRType type;
        private string id;
        private BlockStatement block;
        private Function function;
        private BlockStatement blockStatement;

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

        public Function(IRType type, string id)
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
    }
}
