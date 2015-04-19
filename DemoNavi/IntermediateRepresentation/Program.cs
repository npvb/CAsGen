using DemoNavi.IntermediateRepresentation.Semantic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    public class Program
    {
        public List<DeclarationStatement> Declarations { get; set; }
        public Program(List<DeclarationStatement> declarations)
        {
            this.Declarations = declarations;
            this.SemanticValidation();
        }
        public void SemanticValidation() 
        {
            SemanticContext semanticContext = new SemanticContext();
            semanticContext.FunctionDeclarations = Declarations.OfType<FunctionDeclaration>();

            foreach (var declaration in Declarations)
            {
                declaration.SemanticValidation(semanticContext);
                
            }
            
        }
    }
}
