using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Recompilers.Basic
{
    public class BasicRecompiler : Recompiler
    {
        public override string Recompile(int Export, Program program)
        {
            StringBuilder programBuilder = new StringBuilder();
            programBuilder.AppendFormat("Public Class Class1");
            programBuilder.AppendLine();
            foreach (var declaration in program.Declarations)
            {
                GenerateDeclaration(declaration, programBuilder);
            }
            programBuilder.AppendLine();
            programBuilder.AppendFormat("End Class");
            return programBuilder.ToString();
        }

        private void GenerateDeclaration(DeclarationStatement declaration, StringBuilder programBuilder)
        {
            var declarationType = declaration.GetType();
            if (declaration is FunctionDeclaration)
            {
                GenerateFunction(declaration as FunctionDeclaration, programBuilder);
            }
        }

        private void GenerateFunction(FunctionDeclaration function, StringBuilder programBuilder)
        {
            string parameterFormat = "ByVal {0} As {1}";
            string functionFormat = "Public Function {0}({1}){2}{3}";
            string blockFormat ="{0} End Function";
            var type = function.Type;
            var name = function.Id;

            string typeString = GetTypeString(type);
            if (!(type is VoidType))
            {
                typeString = "As " + typeString;
            }
            string parameters = string.Join(",",function.Parameters.Select(p => string.Format(parameterFormat,p.Id, GetTypeString(p.Type) )));
            string blockString = Environment.NewLine + string.Join(Environment.NewLine, function.Block.StatementList.Select(b => GetStatement(b)));
            blockString = string.Format(blockFormat, blockString);
            programBuilder.Append(string.Format(functionFormat, name, parameters, typeString, blockString));
        }

        private string GetStatement(Statement block)
        {
            if (block is ReturnStatement)
            {
                var statement = block as ReturnStatement;
                string expressionString = GetExpressionString(statement.ReturnExpression);
                return string.Format("Return {0}", expressionString);
            }
            throw new NotImplementedException();
        }

        private string GetExpressionString(Expression expression)
        {
            if( expression is DecValue)
            {
                return expression.ToString();
            }
            throw new NotImplementedException();
        }

        private string GetTypeString(DemoNavi.IntermediateRepresentation.Types.IRType type)
        {
            if (type is IntType)
            {
                return "Int32";
            }
            else if ( type is CharType)
            {
                return "Char";
            }
            return "";
        }
    }
}
