using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Parser
{
    [Serializable]
    public class LexicalErrorException : Exception
    {
        private GOLD.SymbolList symbolList;

        public LexicalErrorException() { }

        public LexicalErrorException(string message) : base(message) { }
        public LexicalErrorException(string message, Exception inner) : base(message, inner) { }
        protected LexicalErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public LexicalErrorException(GOLD.SymbolList symbolList) : this(string.Format("Se esperaba: {0}",symbolList))
        {
            // TODO: Complete member initialization
            this.symbolList = symbolList;
        }

    }
}
