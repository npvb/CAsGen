using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Expressions;
using DemoNavi.IntermediateRepresentation.Statements;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Recompilers.MIPS32
{
    public class Mips32Recompiler : Recompiler
    {
        RegisterFile registerFile = new RegisterFile();

        private void WriteFunction(FunctionDeclaration function, StringBuilder programBuilder)
        {
            programBuilder.AppendFormat("{0}:", GenerateLabel(function));
            programBuilder.AppendLine();
            //reservar espacio en la pila
            WriteBlock(function.Block, programBuilder);
            //restablecer la pila
        }
        private string WriteExpr(Expression expr, RegisterFile registerFile, StringBuilder programBuilder, string registerToUse = "")
        {
            #region INT_EXPRESSIONS
            if (expr is DecValue) 
            {
                var register = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("addi {0}, $zero, {1}", register, expr.ToString());
                programBuilder.AppendFormat(" # agrega {0} al registro {1}", expr.ToString(), register);
                programBuilder.AppendLine();
                return register;

            }
            else if (expr is AddExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as AddExpression;
                var leftRegister = WriteExpr(add.Right,registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("add {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);
                
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("add {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }else if (expr is SubExpression)
            {
                var add = expr as SubExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerFile.FirstAvailableRegister();
                registerFile.FreeRegister(register);
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder);
                programBuilder.AppendFormat("sub {0}, {1}, {2}", register, leftRegister, rightRegister);
                registerFile.FreeRegister(register);
                registerFile.FreeRegister(rightRegister);
                programBuilder.AppendLine();

            }else if (expr is MulExpression)
            {
                var add = expr as MulExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerFile.FirstAvailableRegister();
                registerFile.FreeRegister(register);
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder);
                programBuilder.AppendFormat("mult {0}, {1}, {2}", register, leftRegister, rightRegister); //se hace con los S
                registerFile.FreeRegister(register);
                registerFile.FreeRegister(rightRegister);
                programBuilder.AppendLine();

            }else if (expr is DivisionExpression)
            {
                var add = expr as DivisionExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerFile.FirstAvailableRegister();
                registerFile.FreeRegister(register);
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder);
                programBuilder.AppendFormat("div {0}, {1}, {2}", register, leftRegister, rightRegister);//se hace con los S
                registerFile.FreeRegister(register);
                registerFile.FreeRegister(rightRegister);
                programBuilder.AppendLine();

            }
            #endregion

            #region BITWISE_OP_EXPRESSIONS
            else if (expr is BitwiseAndExpression)
            {
                var add = expr as BitwiseAndExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerFile.FirstAvailableRegister();
                registerFile.FreeRegister(register);
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder);
                programBuilder.AppendFormat("and {0}, {1}, {2}", register, leftRegister, rightRegister);
                registerFile.FreeRegister(register);
                registerFile.FreeRegister(rightRegister);
                programBuilder.AppendLine();

            }
            else if (expr is BitwiseOrExpression)
            {
                var add = expr as BitwiseOrExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerFile.FirstAvailableRegister();
                registerFile.FreeRegister(register);
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder);
                programBuilder.AppendFormat("or {0}, {1}, {2}", register, leftRegister, rightRegister);
                registerFile.FreeRegister(register);
                registerFile.FreeRegister(rightRegister);
                programBuilder.AppendLine();

            }
            else if (expr is BitwiseXorExpression)
            {
                var add = expr as BitwiseXorExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerFile.FirstAvailableRegister();
                registerFile.FreeRegister(register);
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder);
                programBuilder.AppendFormat("xor {0}, {1}, {2}", register, leftRegister, rightRegister);
                registerFile.FreeRegister(register);
                registerFile.FreeRegister(rightRegister);
                programBuilder.AppendLine();

            }
            #endregion

            #region ASSIGN_EXPRESSIONS
            else if (expr is AdditionAssignmentExpression)
            {
                var add = expr as AdditionAssignmentExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("add $t0, $zero, {0}", leftRegister);
                programBuilder.AppendLine();

            }else if (expr is AssignExpression)
            {
                var assign = expr as AssignExpression;
                var assignRegister = WriteExpr(assign.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("add $s0, $zero, {0}", assignRegister); //se puede hacer todas estas operaciones en t0?
                programBuilder.AppendFormat(" # Asigna el resultado de {0} a $t0.", expr.ToString());
                programBuilder.AppendLine();
            }
            #endregion

            #region FUNTIONCALL_EXPRESSION
            else if (expr is FunctionCallExpression)
            {
                WriteFunctionCallExp(expr as FunctionCallExpression,programBuilder);
            }
            #endregion

            return registerFile.FirstAvailableRegister();
        }   
        private void WriteFunctionCallExp(FunctionCallExpression functioncall, StringBuilder programBuilder)
        {
            for (int i = 0; i < functioncall.Parameters.Exprlist.Count; i++)
            {
                programBuilder.AppendFormat("add $a0,$a0, {0}", GetTypeSizeInBytes(functioncall.FunctionDeclaration.Parameters[i].Type));
                programBuilder.AppendLine();
                WriteExpr(functioncall.Parameters.Exprlist[i],registerFile, programBuilder);
                programBuilder.AppendLine("sw $t0,($a0)");
            }
            
        }
        private void WriteReturnStatement(ReturnStatement returnStatement, StringBuilder programBuilder) 
        {
            WriteExpr(returnStatement.ReturnExpression, registerFile, programBuilder);
            programBuilder.AppendLine("add $v0, $t0, $zero");
        }
        private void WriteExpressionStatemnt(ExpressionStatement expressionStatement, StringBuilder programBuilder) 
        {
            RegisterFile registerFile = new RegisterFile();
            WriteExpr(expressionStatement.Expression, registerFile, programBuilder);
            
        }

        private void WriteBlock(BlockStatement blockstatement, StringBuilder programBuilder) 
        {
            foreach (Statement statement in blockstatement.StatementList) 
            {
                if(statement is ReturnStatement)
                {
                    WriteReturnStatement(statement as ReturnStatement, programBuilder);

                }
                else if (statement is ExpressionStatement) 
                {
                    WriteExpressionStatemnt(statement as ExpressionStatement, programBuilder);
                }
               
            }
        }

        #region EXPORT_MARS
        private string GenerateLabel(FunctionDeclaration function) 
        {
            return "_" + function.Id + string.Join("_",function.Parameters.Select(p => p.Type.ToString()));  
        }
        private int GetTypeSizeInWords(IRType type)
        {
            if (type is IntType)
            {
                return 1; 
            }
            else if (type is StructType)
            {
                var strucType = type as StructType;
                return strucType.DeclarationStatement.OfType<IdDeclarationStatement>().Sum(dec => GetTypeSizeInWords(dec.Type));
            }
            return 0;
        }
        private int GetTypeSizeInBytes(IRType type)
        {
            return GetTypeSizeInWords(type) * 4;
        }

        private void WriteDataHeader(Program program, StringBuilder programBuilder) 
        {
            programBuilder.AppendLine(".data");
            programBuilder.AppendFormat("privateMem: .word 0: {0}", program.Declarations.OfType<FunctionDeclaration>().Max(f => f.Parameters.Sum(p => GetTypeSizeInWords(p.Type))));
            programBuilder.AppendLine();

        }
        private void WriteTextHeader(Program program, StringBuilder programBuilder) 
        {
            programBuilder.AppendLine(".text");
            programBuilder.AppendLine("la $a0, privateMem");
            programBuilder.AppendLine("jal _main");
        }
        private void WriteGlobalVariables(Program program, StringBuilder programBuilder)
        {
            programBuilder.AppendLine("#Escribir Variables Globales"); 
        }
        #endregion

        public override string Recompile(int export, Program program)
        {
            StringBuilder programBuilder = new StringBuilder();

            if (export == 1)
            {
                WriteDataHeader(program, programBuilder);
                WriteGlobalVariables(program, programBuilder);
                WriteTextHeader(program, programBuilder);
            }
               
            WriteFunctions(program.Declarations.OfType<FunctionDeclaration>(), programBuilder);
            return programBuilder.ToString();
        }

        private void WriteFunctions(IEnumerable<FunctionDeclaration> functions, StringBuilder programBuilder)
        {
            foreach (var function in functions)
            {
                WriteFunction(function,programBuilder);
            }
        }
    }
}
