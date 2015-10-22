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

namespace DemoNavi.Recompilers.MIPS32
{
    public class Mips32Recompiler : Recompiler
    {
        RegisterFile registerFile = new RegisterFile();
        bool isArray = false;
        bool inStack = false;
        bool param = false;
        bool recursive = false;
        string savedValue = null;
        StringBuilder headerBuilder = new StringBuilder();

        public override string Recompile(Program program)
        {
            StringBuilder programBuilder = new StringBuilder();
            //WriteDataHeader(program, headerBuilder);
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
            registerFile.listofFunctions.Add(function.Id);
            programBuilder.AppendLine();
            //reservar espacio en la pila
            if (GenerateLabel(function) != "_default_main" && function.Parameters.Count()>0)
            {
                programBuilder.AppendFormat("\taddi $sp, $sp, -{0}", function.Parameters.Count() * 2);
                programBuilder.Append("\t# Reserva el espacio de memoria en pila. ");
                programBuilder.AppendLine();
                programBuilder.Append("\tsw $ra, ($sp)");
                programBuilder.Append("\t\t# Guarda el espacio de memoria en pila. ");
                programBuilder.AppendLine();

                if(function.Parameters.Count()>0)
                {
                    for (int x = 0; x < registerFile.savedArguments.Count; x++)
                    {
                        if (registerFile.savedArguments.ElementAt(x).Value == null)
                        {
                           registerFile.savedArguments[registerFile.savedArguments.ElementAt(x).Key] = function.Parameters.ElementAt(x).Id.ToString();
                        }
                        
                    }
                }

                WriteBlock(function.Block, programBuilder);

                programBuilder.AppendFormat("\taddi $sp, $sp, {0}", function.Parameters.Count() * 2);
                programBuilder.Append("\t# Restablece la pila.");
                programBuilder.AppendLine();
                programBuilder.Append("\tjr $ra");
                programBuilder.AppendLine();
            }
            else 
            {
                WriteBlock(function.Block, programBuilder);
            }
        }

        private void WriteFunctionCallExp(FunctionCallExpression functioncall, StringBuilder programBuilder)
        {
            foreach (var id in registerFile.listofFunctions)
            {
                if (id == functioncall.Id) 
                {
                    recursive = true; break;
                }
            }
            if (recursive == false)
            {
                WriteFunctionDataHeader(functioncall, headerBuilder);
                for (int i = 0; i < functioncall.Parameters.Exprlist.Count; i++)
                {
                    var register = registerFile.FirstAvailableArgument();
                    programBuilder.AppendFormat("\tlw {0}, num{1}", register, i.ToString());
                    programBuilder.AppendLine();
                    registerFile.savedArguments.Add(register, null);
                    param = true;
                }
                registerFile.FreeAllArgument();
                programBuilder.AppendFormat("\tjal {0}", GenerateLabel(functioncall.Id, functioncall.Parameters.Exprlist));
                programBuilder.AppendLine();
            }
            else 
            {
                foreach (var param in functioncall.Parameters.Exprlist)
                {
                    WriteExpr(param, registerFile, programBuilder); 
                }
                
            }      
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
            programBuilder.AppendFormat("\tmove $v0, {0}", reg);
            programBuilder.AppendLine();
        }

        private void WriteExpressionStatemnt(ExpressionStatement expressionStatement, StringBuilder programBuilder)
        {
            WriteExpr(expressionStatement.Expression, registerFile, programBuilder);
        }

        private void WriteIdDeclarationStatement(IdDeclarationStatement idDeclarationStatement, StringBuilder programBuilder)
        {
           var reg = WriteExpr(idDeclarationStatement.InitializationExpression, registerFile, programBuilder);
           if (reg == "")
           {
               reg = registerFile.FirstAvailableSavedValue();

               registerFile.stack1.value = "null";//guardar los valores en la pila
               registerFile.stack1.id = idDeclarationStatement.Id;
               registerFile.savedValuesList.Add(reg, registerFile.stack1);
           }
           else 
           {
               registerFile.savedTemporals.Add(reg, idDeclarationStatement.Id);
           }
        }

        private void WriteIFStatement(IfStatement ifStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("_ELSE: ");
            programBuilder.AppendLine();
            var register = WriteExpr(ifStatement.Expressions, registerFile, programBuilder);
            WriteBlock(ifStatement.Statements as BlockStatement, programBuilder);

            if (ifStatement.IfFalse != null)
            {
              /*  programBuilder.AppendFormat("\tbeq {0}, $0, _ELSE", register);
                programBuilder.Append("\t# Compara para retornar al ciclo");
                programBuilder.AppendLine();*/
                
                programBuilder.AppendFormat("\tbeq {0}, 0, _EndIf", register);
                programBuilder.Append("\t# if FALSO continua");
                programBuilder.AppendLine();

                WriteBlock(ifStatement.IfFalse as BlockStatement, programBuilder);
                programBuilder.Append("\tj _ELSE");
                programBuilder.AppendLine();
                programBuilder.Append("_EndIf:");
                programBuilder.AppendLine();


            }
            else
            {
                //programBuilder.Append("_ELSE: ");
                //programBuilder.AppendLine();
                WriteBlock(ifStatement.Statements as BlockStatement, programBuilder);
                programBuilder.Append("\tj _EndIf");
                programBuilder.Append("\t\t# Salta al label _EndIf para finalizar IF");
                programBuilder.AppendLine();
                programBuilder.Append("_EndIf:");
                programBuilder.AppendLine();
            }

           
        }

        private void WriteForStatement(ForStatement forStatement, StringBuilder programBuilder)
        {
            int loopCount = 0;
            loopCount += 1;
            var register = WriteExpr(forStatement.Init, registerFile, programBuilder);
            programBuilder.AppendFormat("_Loop{0}: ",loopCount);
            programBuilder.AppendLine();
            WriteBlock(forStatement.Body as BlockStatement, programBuilder);
            WriteExpr(forStatement.Loop, registerFile, programBuilder);
            var conditionRegister = WriteExpr(forStatement.Condition, registerFile, programBuilder);
            programBuilder.AppendFormat("\tbne {0}, {1}, _Loop{2}", register, conditionRegister,loopCount);
            programBuilder.AppendFormat("\t# Si  {0} es verdadera, sigue el ciclo", forStatement.Condition.ToString());
            programBuilder.AppendLine();

        }

        private void WriteWhileStatement(WhileStatement whileStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("_Loop: ");
            programBuilder.AppendLine();
            var register = WriteExpr(whileStatement.Expressions, registerFile, programBuilder);
            programBuilder.AppendFormat("\tbne {0}, 0, Loop", register);
            programBuilder.AppendFormat("\t# Si  {0} es verdadera, sigue el ciclo", whileStatement.Expressions.ToString());
            programBuilder.AppendLine();
            WriteBlock(whileStatement.Statements as BlockStatement, programBuilder);
            programBuilder.Append("\tj _Loop");
            programBuilder.Append("\t\t\t# Regresa al loop");
            programBuilder.AppendLine();
        }

        private void WriteDoStatement(DoStatement doStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("_Loop: ");
            programBuilder.AppendLine();
            WriteBlock(doStatement.Statements as BlockStatement, programBuilder);
            var register = WriteExpr(doStatement.Expressions, registerFile, programBuilder);
            programBuilder.AppendFormat("\tbne {0}, 0, Loop", register);
            programBuilder.AppendFormat("\t# Si  {0} es verdadera, sigue el ciclo", doStatement.Expressions.ToString());
            programBuilder.AppendLine();
            programBuilder.Append("\tj _Loop");
            programBuilder.Append("\t\t# Regresa al loop");
            programBuilder.AppendLine();
        }

        private void WriteSwitchStatement(SwitchStatement switchStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("_JTAB:\t");
            programBuilder.AppendLine();

            for (int i = 0; i < switchStatement.CaseStatements.Count(); i++)
            {
                programBuilder.AppendFormat("\t.word _Lbl_{0} ", i.ToString());
                programBuilder.AppendLine();
            }
            var register = registerFile.FirstAvailableRegister();
            programBuilder.AppendFormat("la {0}, _JTAB", register);
            programBuilder.Append("\t\t# Carga la dirección inicial de la jumping table.");
            programBuilder.AppendLine();
            programBuilder.AppendFormat("add {0}, {1}, $s1", register, register);
            programBuilder.Append("\t# Suma el offset de la dirección de la tabla.");
            programBuilder.AppendLine();
            var nextRegister = registerFile.FirstAvailableRegister();
            programBuilder.AppendFormat("lw {0}, 0 ({1})", nextRegister, register);
            programBuilder.Append("\t\t# Carga la dirección guardada en _JTAB + $s1");
            programBuilder.AppendLine();
            programBuilder.AppendFormat("jr {0}", nextRegister);
            programBuilder.Append("\t\t\t# y salta a esa dirección.");
            programBuilder.AppendLine();
            registerFile.FreeRegister(register);
            registerFile.FreeRegister(nextRegister);

            for (int i = 0; i<switchStatement.CaseStatements.Count(); i++)
            {
                programBuilder.AppendLine();
                programBuilder.AppendFormat("_Lbl_{0}: ", i.ToString() );
                programBuilder.AppendLine();
                //WriteExpr(switchStatement.Expressions, registerFile, programBuilder);
                BlockStatement cases = new BlockStatement(switchStatement.CaseStatements[i].StatementList);
                WriteBlock(cases, programBuilder);
            }
            programBuilder.AppendLine();
            programBuilder.Append("_EXIT: ");
        }

        private void WriteBreakStatement(BreakStatement breakStatement, StringBuilder programBuilder)
        {
            programBuilder.Append("\tj _EXIT");
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

            }else if (expr is DecValue) 
            {
                var register = expr.ToString();

                if (inStack == true)
                {
                    for (int x = 0; x < registerFile.savedValuesList.Count; x++)
                    {
                        if (registerFile.savedValuesList.ElementAt(x).Key == savedValue)
                        {
                            registerFile.stack1.id = registerFile.savedValuesList.ElementAt(x).Value.id;
                            registerFile.stack1.value = expr.ToString();
                            registerFile.savedValuesList[registerFile.savedValuesList.ElementAt(x).Key] = registerFile.stack1;
                        }
                        
                    }
                   register = savedValue;
                }
                else if (param == true) 
                {
                    for (int x = 0; x < registerFile.savedArguments.Count; x++)
                    {
                        if (registerFile.savedArguments.ElementAt(x).Value == expr.ToString())
                        {
                            register = registerFile.savedArguments.ElementAt(x).Key;
                        }
                    }

                }else
                {
                    register = registerFile.FirstAvailableRegister();
                    
                }

                programBuilder.AppendFormat("\taddi {0}, $zero, {1}", register, expr.ToString());
              programBuilder.AppendFormat("\t# Agrega {0} al registro {1}", expr.ToString(), register);
              programBuilder.AppendLine();
              return register;

            }
            else if(expr is IdValue)
            {
                var register =  registerFile.FirstAvailableSavedValue();
                if (inStack == true && param == false)
                {
                    if (!string.IsNullOrEmpty(registerFile.FirstAvailableSavedValue()))
                    {
                        for (int x = 0; x < registerFile.savedValuesList.Count; x++)
                        {
                            if (registerFile.savedValuesList.ElementAt(x).Key == savedValue)
                            {
                                registerFile.stack1.id = registerFile.savedValuesList.ElementAt(x).Value.id;
                                registerFile.stack1.value = expr.ToString();
                                registerFile.savedValuesList[registerFile.savedValuesList.ElementAt(x).Key] = registerFile.stack1;
                                register = registerFile.savedValuesList.ElementAt(x).Key;
                            }
                        }
                    }
                }
                else if (inStack == false && param == false)
                {
                    for (int x = 0; x < registerFile.savedTemporals.Count; x++)
                    {
                        if (registerFile.savedTemporals.ElementAt(x).Value == expr.ToString())
                        {
                            register = registerFile.savedTemporals.ElementAt(x).Key;
                        }
                    }
                }else if(param == true)
                {
                    for (int x = 0; x < registerFile.savedArguments.Count; x++)
                    {
                        if (registerFile.savedArguments.ElementAt(x).Value == expr.ToString())
                        {
                            register = registerFile.savedArguments.ElementAt(x).Key;
                        }
                    }
                }

                return register;
            }
            else if (expr is AddExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0",registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as AddExpression;
                var leftRegister = WriteExpr(add.Right,registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tadd {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# {0} + {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);
                registerFile.FreeArgument(leftRegister);
                registerFile.FreeSavedValue(leftRegister);
                
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tadd {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendFormat("\t# {0} + {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                    registerFile.FreeArgument(rightRegister);
                    registerFile.FreeSavedValue(rightRegister);
                }
                return register;

            }else if (expr is SubExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as SubExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tsub {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);
                registerFile.FreeArgument(leftRegister);
                registerFile.FreeSavedValue(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tsub {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                    registerFile.FreeArgument(rightRegister);
                    registerFile.FreeSavedValue(rightRegister);
                }
                return register;

            }else if (expr is MulExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as MulExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tmult {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tmult {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }else if (expr is DivisionExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as DivisionExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tdiv {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tdiv {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }
            else if (expr is ModExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as ModExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tdiv {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                register = leftRegister;
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tdiv {0}, {1}",  rightRegister, leftRegister);
                    programBuilder.AppendLine();
                    programBuilder.AppendFormat("\tmove {0}, {1}", rightRegister, register);
                    programBuilder.AppendLine();
                    programBuilder.AppendFormat("\tmfhi {0}", register);
                    programBuilder.AppendFormat("\t# Mueve el HI de {0}", register);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }
            #endregion

            #region BITWISE_OP_EXPRESSIONS
            else if (expr is BitwiseAndExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as BitwiseAndExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tand {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tand {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }
            else if (expr is BitwiseOrExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as BitwiseOrExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tor {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tor {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }
            else if (expr is BitwiseXorExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as BitwiseXorExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\txor {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\txor {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }
            else if (expr is LessThanExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var slt = expr as LessThanExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tslt {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} < {1} ", slt.Left.ToString(),slt.Right.ToString());
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tslt {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;
            }
            else if (expr is LessOrEqualToExpression)
            {
              /*  if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var slt = expr as LessOrEqualToExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("ble {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} < {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("ble {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;*/
                var slt = expr as LessOrEqualToExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                // var register = registerToUse;
                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder);
                var register = rightRegister;
                programBuilder.AppendFormat("\tble {0}, {1}, _BLE", rightRegister, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} > {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                programBuilder.AppendLine("_BLE: ");
                

                registerFile.FreeRegister(leftRegister);
                registerFile.FreeArgument(leftRegister);
                registerFile.FreeSavedValue(leftRegister);
                registerFile.FreeRegister(rightRegister);
                registerFile.FreeArgument(rightRegister);
                registerFile.FreeSavedValue(rightRegister);

                return register;
            }
            else if (expr is GreaterThanExpression)
            {
               /* if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }*/
                var slt = expr as GreaterThanExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
               // var register = registerToUse;
                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder);
                programBuilder.AppendFormat("\tbgt {0}, {1}, _BGT", leftRegister, rightRegister);
                programBuilder.AppendFormat("\t# Evalua {0} > {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                programBuilder.AppendLine("_BGT: ");

                registerFile.FreeRegister(leftRegister);
                registerFile.FreeArgument(leftRegister);
                registerFile.FreeSavedValue(leftRegister);
                registerFile.FreeRegister(rightRegister);
                registerFile.FreeArgument(rightRegister);
                registerFile.FreeSavedValue(rightRegister);

              /*  var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("bgt {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                    registerFile.FreeArgument(rightRegister);
                    registerFile.FreeSavedValue(rightRegister);
                }*/
                return leftRegister;
            }
            else if (expr is GreaterOrEqualExpression)
            {
                /*if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var slt = expr as GreaterThanExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("bge {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} > {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("bge {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;*/
                var slt = expr as GreaterOrEqualExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                // var register = registerToUse;
                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder);
                var register = rightRegister;
                programBuilder.AppendFormat("\tbge {0}, {1}, _BGE", rightRegister, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} > {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                programBuilder.AppendLine("_BGE: ");

                registerFile.FreeRegister(leftRegister);
                registerFile.FreeArgument(leftRegister);
                registerFile.FreeSavedValue(leftRegister);
                registerFile.FreeRegister(rightRegister);
                registerFile.FreeArgument(rightRegister);
                registerFile.FreeSavedValue(rightRegister);

                return register;
            }
            else if (expr is EqualsExpression)
            {
              /*  if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var slt = expr as EqualsExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tbeq {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} < {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tbeq {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;*/
                var slt = expr as EqualsExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                // var register = registerToUse;
                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder);
                var register = rightRegister;
                programBuilder.AppendFormat("\tbeq {0}, {1}, _BEQ", rightRegister, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} == {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                programBuilder.AppendLine("_BEQ: ");

                registerFile.FreeRegister(leftRegister);
                registerFile.FreeArgument(leftRegister);
                registerFile.FreeSavedValue(leftRegister);
                registerFile.FreeRegister(rightRegister);
                registerFile.FreeArgument(rightRegister);
                registerFile.FreeSavedValue(rightRegister);

                return register;


            }
            #endregion

            #region LOGICAL_EXPR
            else if (expr is LogicalOrExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var slt = expr as LogicalOrExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tor {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} < {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tor {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }
            else if (expr is LogicAndExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("\tadd {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var slt = expr as LogicAndExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\tand {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} < {1} ", slt.Left.ToString(), slt.Right.ToString());
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\tand {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;
            #endregion

            #region ASSIGN_EXPRESSIONS
            }
            else if (expr is AdditionAssignmentExpression)
            {
                var add = expr as AdditionAssignmentExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("\taddi $t0, {0}, {1}", leftRegister, add.Left);
                programBuilder.AppendFormat("\t# Inicializar la variable {0} con {1}", leftRegister, add.Left);
                programBuilder.AppendLine();

            }
            else if (expr is SubtractionAsigExpression)
            {
                var add = expr as SubtractionAsigExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("\tsub $t0, $zero, {0}", leftRegister);
                programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", leftRegister);
                programBuilder.AppendLine();

            }
            else if (expr is MultiplicationAsigExpression)
            {
                var add = expr as MultiplicationAsigExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("\tmul $t0, $zero, {0}", leftRegister);
                programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", leftRegister);
                programBuilder.AppendLine();

            }
            else if (expr is DivisionAsigExpression)
            {
                var add = expr as DivisionAsigExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("\tmul $t0, $zero, {0}", leftRegister);
                programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", leftRegister);
                programBuilder.AppendLine();

            }
            else if (expr is AssignExpression)
            {
                var assign = expr as AssignExpression;

                foreach (var id in registerFile.savedValuesList)
                {
                    if (id.Value.id == assign.Left.ToString())
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
                        programBuilder.AppendFormat("\tadd $t0, $zero, {0}", assignRegister);
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
                var postIncrement = expr as PostIncrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("\taddi {0}, {1}, 1", register, register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
                return register;
            }
            else if (expr is PreIncrementExpression)
            {
                var postIncrement = expr as PreIncrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("\taddi {0}, {1}, 1", register, register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
                return register;
            }
            else if (expr is PostDecrementExpression)
            {
                var postIncrement = expr as PostDecrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("\tsubi {0}, {1}, 1", register, register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
                return register;
            }
            else if (expr is PreDecrementExpression)
            {
                var postIncrement = expr as PreDecrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("\tsubi {0}, {1}, 1", register, register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
                return register;
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
                var array = expr as PointerArrayAccessExpr;

                programBuilder.AppendFormat("{0}:	.word {1}", array.Pointer.ToString(), "1000");
                programBuilder.AppendLine();
                var listAddress = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("\tla {0}, {1}", listAddress, array.Pointer.ToString());
                programBuilder.AppendFormat("\t\t# Pone la dirección de la lista en {0}", listAddress);
                programBuilder.AppendLine();
                //registerFile.FreeRegister(listAddress);
                var index = registerFile.FirstAvailableRegister();
                string arrayValue = array.Value.ToString();

                foreach (var id in registerFile.savedValuesList)
                {
                    if (id.Value.id == arrayValue.ToString())
                    {
                        arrayValue = id.Value.value;
                    }
                }


                programBuilder.AppendFormat("\tli {0}, {1}", index, arrayValue);
                programBuilder.AppendFormat("\t\t# Agrega el indice a {0}", index);
                programBuilder.AppendLine();
                var tempReg = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("\tmul {0}, {1}, {2}", tempReg, index, arrayValue);
                programBuilder.AppendFormat("\t\t# {0} es el offset", tempReg);
                programBuilder.AppendLine();
                registerFile.FreeRegister(tempReg);
                tempReg = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("\tadd {0}, {1}, {2}", tempReg, tempReg, listAddress);
                programBuilder.AppendFormat("\t# {0} es la dirección de {1}[{0}]", tempReg, array.Pointer, arrayValue);
                programBuilder.AppendLine();
                /* programBuilder.AppendFormat("add {0}, {1}, {2}", index, index, index);
               programBuilder.Append("\t# Dobla el índice");
               programBuilder.AppendLine();
               programBuilder.AppendFormat("add {0}, {1}, {2}", index, index, index);
               programBuilder.Append("\t# Dobla el índice nuevamente (por 4x)");
               programBuilder.AppendLine();
               var indexAddress = registerFile.FirstAvailableRegister();
               programBuilder.AppendFormat("add {0}, {1}, {2}", indexAddress, index, listAddress);
               programBuilder.Append("\t# Combina los dos elementos de la dirección.");
               programBuilder.AppendLine();*/
                registerFile.FreeRegister(index);
                var valueAddress = registerFile.FirstAvailableRegister();
                //programBuilder.AppendFormat("li {0}, {1}", valueAddress, array.Value);
                programBuilder.AppendFormat("\t\t\t# {0} es el valor {1} que se guardará en {2}[{3}]", valueAddress, array.Value, array.Pointer, array.Value);
                programBuilder.AppendLine();
                programBuilder.AppendFormat("\tsw {0}, 0({1})", valueAddress, tempReg);
                programBuilder.Append("\t\t# Toma el valor indexado en el arreglo.");
                programBuilder.AppendLine();
                //registerFile.FreeRegister(indexAddress);
                registerFile.FreeRegister(tempReg);
                registerFile.FreeRegister(valueAddress);
                return listAddress;
            }
            #endregion

            return registerFile.FirstAvailableRegister();
        }

        #region EXPORT_TO_MARS
        private string GenerateLabel(FunctionDeclaration function) 
        {
            return "_" + function.Id + string.Join("_",function.Parameters.Select(p => p.Type.ToString()));  
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

     /*   private void WriteDataHeader(Program program, StringBuilder programBuilder) 
        {
            programBuilder.AppendLine(".data");

            if (program.Declarations.OfType<FunctionDeclaration>().Select(f => f.Parameters).Count() > 0)//program.Declarations.OfType<FunctionDeclaration>().Max(f => f.Parameters.Sum(p => GetTypeSizeInWords(p.Type))) != 0)
            {
               // programBuilder.AppendFormat("privateMem: .word 0: {0}", program.Declarations.OfType<FunctionDeclaration>().Max(f => f.Parameters.Sum(p => GetTypeSizeInWords(p.Type))));
                
            }
            programBuilder.AppendLine();

        }*/
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
                programBuilder.AppendLine(".data");
            }
            programBuilder.AppendLine(".text");
        }
        private void WriteGlobalVariables(Program program, StringBuilder programBuilder)
        {
           // programBuilder.AppendLine(".globl main "); 
        }

        private void WritetoLog(StringBuilder headerBuilder, StringBuilder programBuilder)
        {
            CreateResultFolder("Ejemplos CasGen");
            
            int count = 1;
            string pathNfile = "Ejemplo_Mips.asm";
            string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Ejemplos CasGen\" + pathNfile;
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                pathNfile = tempFileName;
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
                info.Create();

            FileStream archivoFtp = new FileStream(newFullPath, FileMode.Append);
            StreamWriter escritor = new StreamWriter(archivoFtp, Encoding.Default);

            escritor.WriteLine(headerBuilder);
            escritor.WriteLine(programBuilder);
            escritor.Close();
        }

        public void CreateResultFolder(string name)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (!(System.IO.Directory.Exists(desktopPath + @"\" + name)))
            {
                System.IO.Directory.CreateDirectory(desktopPath + @"\" + name);
            }

        }

        #endregion
    }
}

