using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Expressions;
using DemoNavi.IntermediateRepresentation.Statements;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DemoNavi.Recompilers.x86
{
    public class x86Recompiler : Recompiler
    {
        RegisterFile registerFile = new RegisterFile();
        bool isArray = false;
        bool inStack = false;
        bool param = false;
        string savedValue = null;
        StringBuilder headerBuilder = new StringBuilder();

        public override string Recompile(Program program)
        {
            StringBuilder programBuilder = new StringBuilder();
            registerFile.stackx86.idValue = "-1";
            registerFile.stackx86.positioninStack = "0";
            registerFile.stack.Add("ra", registerFile.stackx86);
            programBuilder.AppendLine("start: ");
            WriteFunctions(program.Declarations.OfType<FunctionDeclaration>(), programBuilder);
            WriteTextHeader(program, headerBuilder);
            WriteGlobalVariables(program, headerBuilder);
            WritetoLog(headerBuilder, programBuilder);


            return programBuilder.ToString();
        }

        private void WriteFunctions(IEnumerable<FunctionDeclaration> functions, StringBuilder programBuilder)
        {
            foreach (var function in functions)
            {
                WriteFunction(function, programBuilder);
            }
        }

        #region Functions
        private void WriteFunction(FunctionDeclaration function, StringBuilder programBuilder)
        {
            programBuilder.AppendFormat("{0}:", GenerateLabel(function));
            programBuilder.AppendLine();
            //reservar espacio en la pila
            if (GenerateLabel(function) != "_default_main" && function.Parameters.Count() > 0)
            {
                programBuilder.Append("push ebp");
                programBuilder.Append("\t# Reserva el espacio de memoria en pila. ");
                programBuilder.AppendLine();
                programBuilder.Append("mov ebp, esp");
                programBuilder.Append("\t\t# Guarda el espacio de memoria en pila. ");
                programBuilder.AppendLine();
                WriteBlock(function.Block, programBuilder);
                programBuilder.AppendFormat("pop ebp");
                programBuilder.Append("\t# Restablece la pila.");
                programBuilder.AppendLine();
                programBuilder.Append("ret");
                programBuilder.AppendLine();
            }
            else
            {
                WriteBlock(function.Block, programBuilder);
            }
        }

        private void WriteFunctionCallExp(FunctionCallExpression functioncall, StringBuilder programBuilder)
        {
        }

        #endregion

        private void WriteBlock(BlockStatement blockstatement, StringBuilder programBuilder)
        {
            foreach (Statement statement in blockstatement.StatementList)
            {
                if (statement is ReturnStatement)
                {
                    WriteReturnStatement(statement as ReturnStatement, programBuilder);
                }
                else if (statement is ExpressionStatement)
                {
                    WriteExpressionStatemnt(statement as ExpressionStatement, programBuilder);
                }
                else if (statement is IdDeclarationStatement)
                {
                    WriteIdDeclarationStatement(statement as IdDeclarationStatement, programBuilder);
                }
                else if (statement is IfStatement)
                {
                    WriteIFStatement(statement as IfStatement, programBuilder);
                }
                else if (statement is ForStatement)
                {
                    WriteForStatement(statement as ForStatement, programBuilder);
                }
                else if (statement is WhileStatement)
                {
                    WriteWhileStatement(statement as WhileStatement, programBuilder);
                }
                else if (statement is DoStatement)
                {
                    WriteDoStatement(statement as DoStatement, programBuilder);
                }
                else if (statement is SwitchStatement)
                {
                    WriteSwitchStatement(statement as SwitchStatement, programBuilder);
                }
                else if (statement is BreakStatement)
                {
                    WriteBreakStatement(statement as BreakStatement, programBuilder);
                }
            }
        }

        #region WriteStatements

        private void WriteReturnStatement(ReturnStatement returnStatement, StringBuilder programBuilder)
        {
            var reg = WriteExpr(returnStatement.ReturnExpression, registerFile, programBuilder);
            programBuilder.AppendFormat("add $v0, {0}, $zero", reg);
            programBuilder.AppendLine();
        }

        private void WriteExpressionStatemnt(ExpressionStatement expressionStatement, StringBuilder programBuilder)
        {
            WriteExpr(expressionStatement.Expression, registerFile, programBuilder);
        }

        private void WriteIdDeclarationStatement(IdDeclarationStatement idDeclarationStatement, StringBuilder programBuilder)
        {
           registerFile.stackx86.idValue = idDeclarationStatement.InitializationExpression.ToString();
           var pos = registerFile.stack.Last().Value.positioninStack;
           registerFile.stackx86.positioninStack = Convert.ToString(Convert.ToInt32(pos)+4);
           registerFile.stack.Add(idDeclarationStatement.Id, registerFile.stackx86);
           
           WriteExpr(idDeclarationStatement.InitializationExpression, registerFile, programBuilder);
            
        }

        private void WriteIFStatement(IfStatement ifStatement, StringBuilder programBuilder)
        {
            var register = WriteExpr(ifStatement.Expressions, registerFile, programBuilder);

            if (ifStatement.IfFalse != null)
            {
                programBuilder.AppendFormat("beq {0}, $0, _ELSE", register);
                programBuilder.Append("\t# if FALSO goto ELSE");
                programBuilder.AppendLine();
                programBuilder.Append("_ELSE: ");
                programBuilder.AppendLine();

                WriteBlock(ifStatement.IfFalse as BlockStatement, programBuilder);
            }
            else
            {
                WriteBlock(ifStatement.Statements as BlockStatement, programBuilder);
                programBuilder.Append("_ELSE: ");
                programBuilder.AppendLine();
            }

            programBuilder.Append("j _EndIf");
            programBuilder.Append("\t\t# Salta al label _EndIf para finalizar IF");
            programBuilder.AppendLine();
            programBuilder.Append("_EndIf:");
            programBuilder.AppendLine();
        }

        private void WriteForStatement(ForStatement forStatement, StringBuilder programBuilder)
        {
            var register = WriteExpr(forStatement.Init, registerFile, programBuilder);
            programBuilder.Append("_Loop: ");
            programBuilder.AppendLine();
            WriteBlock(forStatement.Body as BlockStatement, programBuilder);
            WriteExpr(forStatement.Loop, registerFile, programBuilder);
            var conditionRegister = WriteExpr(forStatement.Condition, registerFile, programBuilder);
            programBuilder.AppendFormat("bne {0}, {1}, _Loop", register, conditionRegister);
            programBuilder.AppendFormat("\t# Si  {0} es verdadera, sigue el ciclo", forStatement.Condition.ToString());
            programBuilder.AppendLine();

        }

        private void WriteWhileStatement(WhileStatement whileStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("_Loop: ");
            programBuilder.AppendLine();
            var register = WriteExpr(whileStatement.Expressions, registerFile, programBuilder);
            programBuilder.AppendFormat("bne {0}, 0, Loop", register);
            programBuilder.AppendFormat("\t# Si  {0} es verdadera, sigue el ciclo", whileStatement.Expressions.ToString());
            programBuilder.AppendLine();
            WriteBlock(whileStatement.Statements as BlockStatement, programBuilder);
            programBuilder.Append("j _Loop");
            programBuilder.Append("\t\t\t# Regresa al loop");
            programBuilder.AppendLine();
        }

        private void WriteDoStatement(DoStatement doStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("_Loop: ");
            programBuilder.AppendLine();
            WriteBlock(doStatement.Statements as BlockStatement, programBuilder);
            var register = WriteExpr(doStatement.Expressions, registerFile, programBuilder);
            programBuilder.AppendFormat("bne {0}, 0, Loop", register);
            programBuilder.AppendFormat("\t# Si  {0} es verdadera, sigue el ciclo", doStatement.Expressions.ToString());
            programBuilder.AppendLine();
            programBuilder.Append("j _Loop");
            programBuilder.Append("\t\t# Regresa al loop");
            programBuilder.AppendLine();
        }

        private void WriteSwitchStatement(SwitchStatement switchStatement, StringBuilder programBuilder)
        {
            
        }

        private void WriteBreakStatement(BreakStatement breakStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("j _EXIT");
            programBuilder.Append("\t\t\t# Salta a _EXIT para salir del ciclo");
            programBuilder.AppendLine();
        }
        #endregion

        private string WriteExpr(Expression expr, RegisterFile registerFile, StringBuilder programBuilder, string registerToUse = "")
        {
            #region INT_EXPRESSIONS
            if (expr == null)
            {
                return "";

            }
            else if (expr is DecValue)
            {
                var register = expr.ToString();

                if (inStack == true)
                {
                    for (int x = 0; x < registerFile.stack.Count(); x++)
                    {
                        if (registerFile.stack.ElementAt(x).Key == savedValue)
                        {
                            register = registerFile.stack.ElementAt(x).Value.positioninStack;
                            inStack = true;

                        }
                    }
                     programBuilder.AppendFormat("mov eax, [ebp+{0}]", register);
                     programBuilder.AppendFormat("\t# Agrega {0} al registro {1}", expr.ToString(), register);
                     programBuilder.AppendLine();
                     register = savedValue;  
                }
                else if (param == true)
                {
                }
                else 
                {
                    register = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("mov {0}, {1}", register, expr.ToString());
                    programBuilder.AppendFormat("\t# Agrega {0} al registro {1}", expr.ToString(), register);
                    programBuilder.AppendLine();
                }


                return register;
            }
            else if (expr is IdValue)
            {
                var register = expr.ToString();

                for (int x = 0; x < registerFile.stack.Count(); x++)
                {
                    if(registerFile.stack.ElementAt(x).Key == register)
                    {
                        register = registerFile.stack.ElementAt(x).Value.positioninStack;
                        programBuilder.AppendFormat("mov eax, [ebp+{0}]", register);
                        programBuilder.AppendLine();
                    }
                }
                return register;
            }
            else if (expr is AddExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, 0", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as AddExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("add {0}, {1}",  register, leftRegister);
                programBuilder.AppendFormat("\t# {0} + {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("add {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t# {0} + {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                return register;


            }
            else if (expr is SubExpression)
            {
              
               

            }
            else if (expr is MulExpression)
            {
               
           

            }
            else if (expr is DivisionExpression)
            {
               
             

            }
            #endregion

            #region BITWISE_OP_EXPRESSIONS
            else if (expr is BitwiseAndExpression)
            {
               


            }
            else if (expr is BitwiseOrExpression)
            {
               


            }
            else if (expr is BitwiseXorExpression)
            {
               

            }
            else if (expr is LessThanExpression)
            {
                
            }
            else if (expr is LessOrEqualToExpression)
            {
                
            }
            else if (expr is GreaterThanExpression)
            {
               
            }
            else if (expr is GreaterOrEqualExpression)
            {
               
            }
            else if (expr is EqualsExpression)
            {
                

            }
            #endregion

            #region LOGICAL_EXPR
            else if (expr is LogicalOrExpression)
            {
                

            }
            else if (expr is LogicAndExpression)
            {
               
            #endregion

            #region ASSIGN_EXPRESSIONS
            }
            else if (expr is AdditionAssignmentExpression)
            {


            }
            else if (expr is SubtractionAsigExpression)
            {


            }
            else if (expr is MultiplicationAsigExpression)
            {


            }
            else if (expr is DivisionAsigExpression)
            {

            }
            else if (expr is AssignExpression)
            {
                var assign = expr as AssignExpression;

                foreach (var id in registerFile.stack)
                {
                    if (id.Key == assign.Left.ToString())
                    {
                        inStack = true;
                        savedValue = id.Key;
                    }
                }

                var assignRegister = WriteExpr(assign.Right, registerFile, programBuilder);

                if (assign.Left is PointerArrayAccessExpr)
                {
                    isArray = true;
                    registerFile.FreeRegister(assignRegister);
                    WriteExpr(assign.Left, registerFile, programBuilder);
                    return assignRegister;
                }
                else
                {
                    if (inStack == false)
                    {
                        programBuilder.AppendFormat("mov {0}, {1}", assignRegister, assign.Left.ToString());
                    }
                    programBuilder.AppendLine();
                    inStack = false;
                    return assignRegister;
                }
            }
                #endregion

            #region POST_PRE_INCREMENT_DECREMENT
            else if (expr is PostIncrementExpression)
            {
              
            }
            else if (expr is PreIncrementExpression)
            {
               
            }
            else if (expr is PostDecrementExpression)
            {
               
            }
            else if (expr is PreDecrementExpression)
            {
            }
            #endregion

            #region FUNTIONCALL_EXPRESSION
            else if (expr is FunctionCallExpression)
            {
                WriteFunctionCallExp(expr as FunctionCallExpression, programBuilder);
            }
            #endregion

            #region ARRAY_EXPRESSION
            else if (expr is PointerArrayAccessExpr)
            {
               
            }
            #endregion

            return registerFile.FirstAvailableRegister();
        }

        #region EXPORT_TO_FASM
        private string GenerateLabel(FunctionDeclaration function)
        {
            return "_" + function.Id + string.Join("_", function.Parameters.Select(p => p.Type.ToString()));
        }

        private object GenerateLabel(string id, List<Expression> list)
        {
            return "_" + id + string.Join("_", list.Select(p => p.GetIRType().ToString()));
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

        private void WriteFunctionDataHeader(FunctionCallExpression function, StringBuilder programBuilder)
        {
            int count = 0;
            programBuilder.AppendLine(".data");
            foreach (var parameters in function.Parameters.Exprlist)
            {
                programBuilder.AppendFormat("num{0}: .word  {1}", count, parameters.ToString());
                count += 1;
                programBuilder.AppendLine();
            }
        }

        private void WriteTextHeader(Program program, StringBuilder programBuilder)
        {
            if (param == false)
            {
                programBuilder.AppendLine("section '.data' data readable writeable");
            }
            programBuilder.AppendLine("section '.code' code readable executable");
        }
        private void WriteGlobalVariables(Program program, StringBuilder programBuilder)
        {
            // programBuilder.AppendLine(".globl main "); 
        }

        private void WritetoLog(StringBuilder headerBuilder, StringBuilder programBuilder)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string pathNfile = String.Empty;
            pathNfile = path + "\\Ejemplo_x86.asm";

            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
                info.Create();

            FileStream archivoFtp = new FileStream(pathNfile, FileMode.Append);
            StreamWriter escritor = new StreamWriter(archivoFtp, Encoding.Default);

            escritor.WriteLine(headerBuilder);
            escritor.WriteLine(programBuilder);
            escritor.Close();
        }
        #endregion
    }
}
