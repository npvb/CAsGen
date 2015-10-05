using DemoNavi.IntermediateRepresentation;
using DemoNavi.IntermediateRepresentation.Expressions;
using DemoNavi.IntermediateRepresentation.Statements;
using DemoNavi.IntermediateRepresentation.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Recompilers.x86
{
    public class x86Recompiler : Recompiler
    {
        RegisterFile registerFile = new RegisterFile();
        bool isArray = false;
        bool indexFound = false;
        int labelCount = 0;
        int count;
        bool param = false;
        bool inStack = false;
        bool inCycle = false;
        bool recursive = false;
        string savedValue = null;
        string mainName = null;

        StringBuilder headerBuilder = new StringBuilder();

        public override string Recompile(Program program)
        {
            StringBuilder programBuilder = new StringBuilder();
            registerFile.stackx86.idValue = "-1";
            registerFile.stackx86.positioninStack = "0";
            registerFile.stack.Add("ra", registerFile.stackx86);
            registerFile.savedTemporals.Add("1", true);
            WriteFunctions(program.Declarations.OfType<FunctionDeclaration>(), programBuilder);
            WriteTextHeader(program, headerBuilder);
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
            mainName = GenerateLabel(function);
            if (function.Parameters.Count() > 0)
            {
                foreach (var param in function.Parameters)
                {
                    registerFile.stackx86.idValue = null;
                    var pos = registerFile.stack.Last().Value.positioninStack;
                    registerFile.stackx86.positioninStack = Convert.ToString(Convert.ToInt32(pos) + 4);
                    registerFile.stack.Add(param.Id, registerFile.stackx86);
                }
            }

            WriteGlobalVariables(programBuilder, mainName);
            programBuilder.AppendLine();
            programBuilder.AppendFormat("{0}:", mainName);
            programBuilder.AppendLine();
            programBuilder.Append("\t push ebp");
            programBuilder.Append("\t\t# Reserva el espacio de memoria en pila. ");
            programBuilder.AppendLine();
            programBuilder.Append("\t mov ebp, esp");
            programBuilder.Append("\t\t# Guarda el espacio de memoria en pila. ");
            programBuilder.AppendLine();
            programBuilder.Append("\t sub ebp, -64");
            programBuilder.AppendLine("\t\t# Reserva el espacio para variables a usar. ");

            WriteBlock(function.Block, programBuilder);

            programBuilder.Append("\t mov DWORD PTR [ebp-16], eax");
            programBuilder.Append("\t# Guarda eax en la pila para retorno");
            programBuilder.AppendLine();
            programBuilder.AppendLine("\t leave");
            programBuilder.Append("\t ret");
            programBuilder.Append("\t\t# Restablece la pila.");
            programBuilder.AppendLine();
           
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

                for (int i = 0; i < functioncall.Parameters.Exprlist.Count; i++)
                {
                    WriteExpr(functioncall.Parameters.Exprlist[i], registerFile, programBuilder);
                }
                programBuilder.AppendLine("\t pop ebp");
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
            programBuilder.AppendFormat("\t mov eax, {0}", reg);
            programBuilder.AppendLine();
        }

        private void WriteExpressionStatemnt(ExpressionStatement expressionStatement, StringBuilder programBuilder)
        {
            WriteExpr(expressionStatement.Expression, registerFile, programBuilder);
        }

        private void WriteIdDeclarationStatement(IdDeclarationStatement idDeclarationStatement, StringBuilder programBuilder)
        {
            var pos = registerFile.stack.Last().Value.positioninStack;

            if (idDeclarationStatement.InitializationExpression != null && !(idDeclarationStatement.InitializationExpression is AddExpression))
            {
                registerFile.stackx86.idValue = idDeclarationStatement.InitializationExpression.ToString();
                registerFile.stackx86.positioninStack = Convert.ToString(Convert.ToInt32(pos) + 4);
                savedValue = idDeclarationStatement.Id;
                registerFile.stack.Add(idDeclarationStatement.Id, registerFile.stackx86);
            }
            else
                if (idDeclarationStatement.InitializationExpression == null)
                {
                    // registerFile.stackx86.idValue = null;
                    registerFile.stackx86.positioninStack = Convert.ToString(Convert.ToInt32(pos) + 4);
                    //registerFile.savedTemporals.Add(idDeclarationStatement.Id, true);
                    registerFile.stack.Add(idDeclarationStatement.Id, registerFile.stackx86);
                }
                else
                {
                    savedValue = idDeclarationStatement.Id;
                    registerFile.savedTemporals.Add(savedValue, true);
                }


            WriteExpr(idDeclarationStatement.InitializationExpression, registerFile, programBuilder);

        }

        private void WriteIFStatement(IfStatement ifStatement, StringBuilder programBuilder)
        {
            var register = WriteExpr(ifStatement.Expressions, registerFile, programBuilder);

            if (ifStatement.IfFalse != null)
            {
                /* programBuilder.AppendFormat("beq {0}, $0, _ELSE", register);
                 programBuilder.Append("\t# if FALSO goto ELSE");
                 programBuilder.AppendLine();
                 programBuilder.Append("_ELSE: ");
                 programBuilder.AppendLine();*/

                WriteBlock(ifStatement.IfFalse as BlockStatement, programBuilder);
            }
            else
            {
                WriteBlock(ifStatement.Statements as BlockStatement, programBuilder);
                programBuilder.AppendFormat("jmp .L{0}", labelCount);
                programBuilder.AppendLine();
            }

            programBuilder.AppendFormat(".L{0}: ", labelCount);
            programBuilder.AppendLine();
            /*programBuilder.Append("j _EndIf");
            programBuilder.Append("\t\t# Salta al label _EndIf para finalizar IF");
            programBuilder.AppendLine();
            programBuilder.Append("_EndIf:");
            programBuilder.AppendLine();*/
        }

        private void WriteForStatement(ForStatement forStatement, StringBuilder programBuilder)
        {
            var register = WriteExpr(forStatement.Init, registerFile, programBuilder);
            labelCount += 1;
            programBuilder.AppendFormat("jmp .L{0} ", labelCount);
            programBuilder.AppendLine();
            programBuilder.AppendFormat(".L{0}: ", labelCount);
            programBuilder.AppendLine();
            WriteExpr(forStatement.Condition, registerFile, programBuilder);
            labelCount += 1;
            programBuilder.AppendFormat("jmp .L{0} ", labelCount);
            programBuilder.AppendLine();
            programBuilder.AppendFormat(".L{0}: ", labelCount);
            programBuilder.AppendLine();
            WriteBlock(forStatement.Body as BlockStatement, programBuilder);
            WriteExpr(forStatement.Loop, registerFile, programBuilder);
        }

        private void WriteWhileStatement(WhileStatement whileStatement, StringBuilder programBuilder)
        {
            labelCount += 1;
            programBuilder.AppendFormat("jmp .L{0} ", labelCount);
            programBuilder.AppendLine();
            programBuilder.AppendFormat(".L{0}: ", labelCount);
            programBuilder.AppendLine();
            WriteExpr(whileStatement.Expressions, registerFile, programBuilder);
            WriteBlock(whileStatement.Statements as BlockStatement, programBuilder);
        }

        private void WriteDoStatement(DoStatement doStatement, StringBuilder programBuilder)
        {
            labelCount += 1;
            programBuilder.AppendFormat("jmp .L{0} ", labelCount);
            programBuilder.AppendLine();
            programBuilder.AppendFormat(".L{0}: ", labelCount);
            programBuilder.AppendLine();
            WriteBlock(doStatement.Statements as BlockStatement, programBuilder);
            WriteExpr(doStatement.Expressions, registerFile, programBuilder);

        }

        private void WriteSwitchStatement(SwitchStatement switchStatement, StringBuilder programBuilder)
        {
            var register = WriteExpr(switchStatement.Expressions, registerFile, programBuilder);

            for (int i = 0; i < switchStatement.CaseStatements.Count(); i++)
            {
                if (switchStatement.CaseStatements.ElementAt(i) is CaseStatement)
                {
                    labelCount = i + 1;
                    programBuilder.AppendFormat("\t cmp {0}, {1}", register, (i + 1).ToString());
                    programBuilder.AppendLine();
                    programBuilder.AppendFormat("\t je .L{0} ", (i + 1).ToString());
                    programBuilder.AppendLine();
                }
                else
                    if (switchStatement.CaseStatements.ElementAt(i) is DefaultCaseStatement)
                    {
                        labelCount = switchStatement.CaseStatements.Count() + 1;
                        programBuilder.AppendFormat("\t jne .L{0} ", labelCount);
                        programBuilder.AppendLine();
                    }
            }

            for (int i = 0; i < switchStatement.CaseStatements.Count(); i++)
            {
                programBuilder.AppendFormat(".L{0} ", (i + 1).ToString());
                programBuilder.AppendLine();
                BlockStatement cases = new BlockStatement(switchStatement.CaseStatements[i].StatementList);
                WriteBlock(cases, programBuilder);
            }
            programBuilder.AppendLine();
            programBuilder.AppendFormat(".L{0} ", switchStatement.CaseStatements.Count().ToString());
            programBuilder.AppendLine();
        }

        private void WriteBreakStatement(BreakStatement breakStatement, StringBuilder programBuilder)
        {
            programBuilder.AppendFormat("\t jne .L{0} ", labelCount);
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
                for (int x = 0; x < registerFile.stack.Count(); x++)
                {
                    if (registerFile.stack.ElementAt(x).Key == savedValue)
                    {
                        indexFound = true;
                        //inStack = true;
                        inCycle = true;
                        register = registerFile.RegisterPosition(savedValue);//registerFile.stack.ElementAt(x).Value.positioninStack;

                        registerFile.stackx86.positioninStack = register;
                        registerFile.stackx86.idValue = expr.ToString();
                        registerFile.stack[registerFile.stack.ElementAt(x).Key] = registerFile.stackx86;

                    }
                    else
                        for (int i = 0; i < registerFile.savedTemporals.Count(); i++)
                        {
                            if (registerFile.savedTemporals.ElementAt(i).Key == savedValue)
                            {
                                indexFound = false;
                                register = registerFile.RegisterPosition(savedValue);//registerFile.stack.ElementAt(i).Value.positioninStack;
                                break;
                            }
                        }
                }

                if (indexFound == true && inCycle == true)//inStack == true)
                {
                    programBuilder.AppendFormat("\t mov DWORD PTR [ebp-{0}], {1}", register, expr.ToString());
                    programBuilder.AppendFormat("\t# Mueve {0} a la posición en la pila {1}", expr.ToString(), register);
                    programBuilder.AppendLine();
                    register = string.Format("DWORD PTR [ebp-{0}]", register);
                    indexFound = false;
                }
                else
                    if (indexFound == false || inStack == true || inCycle == true)
                    {
                        var reg = registerFile.FirstAvailableRegister();
                        programBuilder.AppendFormat("\t mov {0} , {1}", reg, expr.ToString());
                        programBuilder.AppendFormat("\t# Mueve {0} a {1}", expr.ToString(), reg);
                        programBuilder.AppendLine();
                        register = reg;

                    }

                return register;
            }
            else if (expr is IdValue)
            {
                var register = expr.ToString();
                var firstAvailableReg = registerFile.FirstAvailableRegister();

                for (int x = 0; x < registerFile.stack.Count(); x++)
                {
                    if (registerFile.stack.ElementAt(x).Key == register)
                    {
                        register = registerFile.stack.ElementAt(x).Value.positioninStack;
                    }
                }
                programBuilder.AppendFormat("\t mov {0}, DWORD PTR [ebp-{1}]", firstAvailableReg, register);
                programBuilder.AppendFormat("\t# Mueve {0} a la pos ebp - {1}", expr.ToString(), register);
                programBuilder.AppendLine();
                register = firstAvailableReg;
                registerFile.FreeRegister(firstAvailableReg);

                return register;
            }
            else if (expr is AddExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as AddExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t add {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} + {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t add {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} + {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                return register;
            }
            else if (expr is SubExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as SubExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t sub {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} - {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t sub {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} - {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                return register;

            }
            else if (expr is MulExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as MulExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t mul {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} * {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t mul {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} * {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                return register;

            }
            else if (expr is DivisionExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as DivisionExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t div {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} / {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t div {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} / {1}", register, rightRegister);
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
                }
                var add = expr as ModExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t mov {0}, {1}", register, leftRegister);
                programBuilder.AppendLine();
                programBuilder.AppendFormat("\t sar {0}, 31", leftRegister);
                programBuilder.AppendLine();
               // register = leftRegister;
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);

                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t idiv {0}", rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} % {1}", register, rightRegister);
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
                }
                var add = expr as ModExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t div {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} % {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t move {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} / {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    programBuilder.AppendFormat("\t mfhi {0}", rightRegister);
                    programBuilder.AppendFormat("\t\t# Pasa el valor {0} para guardar el MOD", rightRegister);
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
                }
                var add = expr as BitwiseAndExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t and {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} & {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t and {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} & {1}", register, rightRegister);
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
                }
                var add = expr as BitwiseOrExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t or {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} | {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t or {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} | {1}", register, rightRegister);
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
                }
                var add = expr as BitwiseXorExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t xor {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t\t# {0} ^ {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t xor {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} ^ {1}", register, rightRegister);
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
                }
                var add = expr as LessThanExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t mov {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t# Mueve {0} a {1} para hacer el cmp", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t cmp {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} < {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                labelCount += 1;
                programBuilder.AppendFormat("\t jge .L{0}", labelCount);
                programBuilder.AppendFormat("\t\t# Salta al label {0} si es >=.", labelCount);
                programBuilder.AppendLine();
                programBuilder.AppendFormat(".L{0}", labelCount);
                programBuilder.AppendLine();

                return register;

            }
            else if (expr is LessOrEqualToExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as LessOrEqualToExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t mov {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t# Mueve {0} a {1} para hacer el cmp", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t cmp {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} < {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                labelCount += 1;
                programBuilder.AppendFormat("\t jg .L{0}", labelCount);
                programBuilder.AppendFormat("\t\t# Salta al label {0} si es >=.", labelCount);
                programBuilder.AppendLine();
                programBuilder.AppendFormat(".L{0}", labelCount);
                programBuilder.AppendLine();

                return register;
            }
            else if (expr is GreaterThanExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as GreaterThanExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t mov {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t# Mueve {0} a {1} para hacer el cmp", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t cmp {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} < {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                labelCount += 1;
                programBuilder.AppendFormat("\t jle .L{0}", labelCount);
                programBuilder.AppendFormat("\t\t# Salta al label {0} si es <.", labelCount);
                programBuilder.AppendLine();
                programBuilder.AppendFormat(".L{0}", labelCount);
                programBuilder.AppendLine();

                return register;
            }
            else if (expr is GreaterOrEqualExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as GreaterOrEqualExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t mov {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t# Mueve {0} a {1} para hacer el cmp", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t cmp {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} < {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                labelCount += 1;
                programBuilder.AppendFormat("\t jl .L{0}", labelCount);
                programBuilder.AppendFormat("\t\t# Salta al label {0} si es <.", labelCount);
                programBuilder.AppendLine();
                programBuilder.AppendFormat(".L{0}", labelCount);
                programBuilder.AppendLine();

                return register;

            }
            else if (expr is EqualsExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as EqualsExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t mov {0}, {1}", register, leftRegister);
                programBuilder.AppendFormat("\t# Mueve {0} a {1} para hacer el cmp", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t cmp {0}, {1}", register, rightRegister);
                    programBuilder.AppendFormat("\t\t# {0} < {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                labelCount += 1;
                programBuilder.AppendFormat("\t jne .L{0}", labelCount);
                programBuilder.AppendFormat("\t\t# Salta al label {0} si es <.", labelCount);
                programBuilder.AppendLine();

                return register;

            }
            #endregion

            #region LOGICAL_EXPR
            else if (expr is LogicalOrExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as BitwiseAndExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t cmp {0}, 0", leftRegister);
                programBuilder.AppendFormat("\t\t# Compara {0} con 0", leftRegister);
                programBuilder.AppendLine();
                programBuilder.AppendFormat("\t jne .L{0}", labelCount);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t cmp {0}, 0", rightRegister);
                    programBuilder.AppendFormat("\t\t# Compara {0} con 0", rightRegister);
                    programBuilder.AppendLine();
                    programBuilder.AppendFormat("\t jne .L{0}", labelCount);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);

                }
                return register;

            }
            else if (expr is LogicAndExpression)
            {
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                }
                var add = expr as BitwiseAndExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("\t cmp {0}, 0", leftRegister);
                programBuilder.AppendFormat("\t\t# Compara {0} con 0", leftRegister);
                programBuilder.AppendLine();
                programBuilder.AppendFormat("\t je .L{0}", labelCount);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);


                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("\t cmp {0}, 0", rightRegister);
                    programBuilder.AppendFormat("\t\t# Compara {0} con 0", rightRegister);
                    programBuilder.AppendLine();
                    programBuilder.AppendFormat("\t je .L{0}", labelCount);
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
                programBuilder.AppendFormat("\t add {0}, {1}", leftRegister, add.Left);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();

            }
            else if (expr is SubtractionAsigExpression)
            {
                var sub = expr as SubtractionAsigExpression;
                var leftRegister = WriteExpr(sub.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("\t sub {0}, {1}", leftRegister, sub.Left);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();

            }
            else if (expr is MultiplicationAsigExpression)
            {
                var sub = expr as MultiplicationAsigExpression;
                var leftRegister = WriteExpr(sub.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("\t mul {0}, {1}", leftRegister, sub.Left);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();

            }
            else if (expr is DivisionAsigExpression)
            {
                var sub = expr as DivisionAsigExpression;
                var leftRegister = WriteExpr(sub.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("\t div {0}, {1}", leftRegister, sub.Left);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
            }
            else if (expr is AssignExpression)
            {
                var assign = expr as AssignExpression;

                foreach (var id in registerFile.stack)
                {
                    if (id.Key == assign.Left.ToString())
                    {
                        savedValue = id.Key;
                        inStack = true;
                    }
                }

                if (inStack == false)
                {
                    var pos = registerFile.stack.Last().Value.positioninStack;
                    registerFile.stackx86.idValue = assign.Right.ToString();
                    savedValue = assign.Left.ToString();
                    registerFile.stackx86.positioninStack = Convert.ToString(Convert.ToInt32(pos) + 4);
                    registerFile.stack.Add(assign.Left.ToString(), registerFile.stackx86);
                    inStack = true;
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
                        programBuilder.AppendFormat("\t mov {0}, {1}", assignRegister, assign.Left.ToString());
                        programBuilder.AppendLine();
                    }

                    inStack = false;
                    return assignRegister;
                }
            }
                #endregion

            #region POST_PRE_INCREMENT_DECREMENT
            else if (expr is PostIncrementExpression)
            {
                var postIncrement = expr as PostIncrementExpression;
                var register = registerFile.RegisterPosition(postIncrement.Expression.ToString());
                programBuilder.AppendFormat("\t add DWORD PTR [epb-{0}], 1", register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();

                return register;
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
        private void WriteGlobalVariables(StringBuilder programBuilder, string functionName)
        {
            programBuilder.AppendLine(".globl main");
            programBuilder.AppendFormat("\t.type {0}, @function", functionName);
            programBuilder.AppendLine();
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
