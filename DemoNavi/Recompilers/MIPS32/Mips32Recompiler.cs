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

        bool isArray = false;
       
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
                WriteFunction(function, programBuilder);
            }
        }

        #region Functions
        private void WriteFunction(FunctionDeclaration function, StringBuilder programBuilder)
        {
            programBuilder.AppendFormat("{0}:", GenerateLabel(function));
            programBuilder.AppendLine();
            //reservar espacio en la pila
            if (GenerateLabel(function) != "_default_main")
            {
                programBuilder.AppendFormat("add $sp, #sp, -{0}", function.Parameters.Count() * 2);
                programBuilder.Append("\t# Reserva el espacio de memoria en pila. ");
                programBuilder.AppendLine();
                programBuilder.Append("sw $t0, 0($sp)");
                programBuilder.Append("\t\t# Guarda el espacio de memoria en pila. ");
                programBuilder.AppendLine();
                WriteBlock(function.Block, programBuilder);
                //restablecer la pila
                programBuilder.Append("lw $t0, 0($sp)");
                programBuilder.Append("\t\t# Carga el valor anterior de los parametros. ");
                programBuilder.AppendLine();
                programBuilder.AppendFormat("add $sp, #sp, {0}", function.Parameters.Count() * 2);
                programBuilder.Append("\t# Restablece la pila.");
                programBuilder.AppendLine();
                programBuilder.Append("jr $ra");
            }
            else 
            {
                WriteBlock(function.Block, programBuilder);
            }
        }

        private void WriteFunctionCallExp(FunctionCallExpression functioncall, StringBuilder programBuilder)
        { 
           for (int i = 0; i < functioncall.Parameters.Exprlist.Count; i++)
            {
                programBuilder.AppendFormat("add $a0,$a0, {0}", GetTypeSizeInBytes(functioncall.Parameters.Exprlist[i].GetIRType()));
                programBuilder.AppendLine();
                var register = WriteExpr(functioncall.Parameters.Exprlist[i], registerFile, programBuilder);
                programBuilder.AppendFormat("sw {0},($a0)", register);
                programBuilder.AppendLine();
                registerFile.FreeRegister(register);
            }
           programBuilder.AppendFormat("jal {0}", GenerateLabel(functioncall.Id, functioncall.Parameters.Exprlist));
           programBuilder.AppendLine();
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
            WriteExpr(returnStatement.ReturnExpression, registerFile, programBuilder);
            programBuilder.AppendLine("add $v0, $t0, $zero");
        }

        private void WriteExpressionStatemnt(ExpressionStatement expressionStatement, StringBuilder programBuilder)
        {
            WriteExpr(expressionStatement.Expression, registerFile, programBuilder);
        }

        private void WriteIdDeclarationStatement(IdDeclarationStatement idDeclarationStatement, StringBuilder programBuilder)
        {
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

            }

            programBuilder.Append("j _EndIf");
            programBuilder.Append("\t\t# Salta al label _EndIf para finalizar IF");
            programBuilder.AppendLine();
            programBuilder.Append("_EndIf:");
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

            }else if (expr is DecValue) 
            {
              var register = registerFile.FirstAvailableRegister();
              programBuilder.AppendFormat("addi {0}, $zero, {1}", register, expr.ToString());
              programBuilder.AppendFormat("\t# Agrega {0} al registro {1}", expr.ToString(), register);
              programBuilder.AppendLine();
              return register;

            }
            else if(expr is IdValue)
            {
                registerFile = new RegisterFile(expr.ToString());
                var argument = registerFile.FirstAvailableArgument();
                programBuilder.AppendFormat("addi {0}, $zero, {1}", argument, expr.ToString());
                programBuilder.AppendFormat("\t# Agrega el valor de {0} al argumento {1}", expr.ToString(), argument);
                programBuilder.AppendLine();
                return argument;
            }
            else if (expr is AddExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0",registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as AddExpression;
                var leftRegister = WriteExpr(add.Right,registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("add {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendFormat("\t# {0} + {1}", register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);
                
                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("add {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendFormat("\t# {0} + {1}", register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }else if (expr is SubExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add{0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as SubExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("sub {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("sub {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }else if (expr is MulExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as MulExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("mult {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("mult {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;

            }else if (expr is DivisionExpression)
            {
                if (string.IsNullOrEmpty(registerToUse))
                {
                    registerToUse = registerFile.FirstAvailableRegister();
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as DivisionExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("div {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("div {0}, {1}, {2}", register, register, rightRegister);
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
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as BitwiseAndExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("and {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("and {0}, {1}, {2}", register, register, rightRegister);
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
                    programBuilder.AppendFormat("add{0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as BitwiseOrExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("or {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("or {0}, {1}, {2}", register, register, rightRegister);
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
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var add = expr as BitwiseXorExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("xor {0}, {1}, {2}", register, register, leftRegister);
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(add.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("xor {0}, {1}, {2}", register, register, rightRegister);
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
                    programBuilder.AppendFormat("add {0}, $zero, $zero", registerToUse);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", registerToUse);
                    programBuilder.AppendLine();
                }
                var slt = expr as LessThanExpression;
                var leftRegister = WriteExpr(slt.Right, registerFile, programBuilder);
                var register = registerToUse;
                programBuilder.AppendFormat("slt {0}, {1}, {2}",register, register, leftRegister);
                programBuilder.AppendFormat("\t# Evalua {0} < {1} ", slt.Left.ToString(),slt.Right.ToString());
                programBuilder.AppendLine();
                registerFile.FreeRegister(leftRegister);

                var rightRegister = WriteExpr(slt.Left, registerFile, programBuilder, register);
                if (register != rightRegister)
                {
                    programBuilder.AppendFormat("slt {0}, {1}, {2}", register, register, rightRegister);
                    programBuilder.AppendLine();
                    registerFile.FreeRegister(rightRegister);
                }
                return register;
            }
            #endregion

            #region ASSIGN_EXPRESSIONS
            else if (expr is AdditionAssignmentExpression)
            {
                var add = expr as AdditionAssignmentExpression;
                var leftRegister = WriteExpr(add.Right, registerFile, programBuilder);
                programBuilder.AppendFormat("add $t0, $zero, {0}", leftRegister);
                programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", leftRegister);
                programBuilder.AppendLine();

            }
            else if (expr is AssignExpression)
            {
                var assign = expr as AssignExpression;

                if (assign.Left is PointerArrayAccessExpr)
                {
                    isArray = true;
                    var assignRegister = WriteExpr(assign.Right, registerFile, programBuilder);
                    WriteExpr(assign.Left, registerFile, programBuilder);
                   // programBuilder.AppendFormat("add $t0, $zero, {0}", assignRegister);
                   // programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", assignRegister);
                   // programBuilder.AppendLine();
                    return assignRegister;
                }
                else 
                {
                    var assignRegister = WriteExpr(assign.Right, registerFile, programBuilder);
                    programBuilder.AppendFormat("add $t0, $zero, {0}", assignRegister);
                    programBuilder.AppendFormat("\t# Inicializar la variable {0} con 0", assignRegister);
                    programBuilder.AppendLine();
                    return assignRegister;
                }

            }
            #endregion

            #region POST_PRE_INCREMENT_DECREMENT
            else if (expr is PostIncrementExpression)
            {
                var postIncrement = expr as PostIncrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("addi {0}, {1}, 1", register, register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
                return register;
            }
            else if (expr is PreIncrementExpression)
            {
                var postIncrement = expr as PreIncrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("addi {0}, {1}, 1", register, register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
                return register;
            }
            else if (expr is PostDecrementExpression)
            {
                var postIncrement = expr as PostDecrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("subi {0}, {1}, 1", register, register);
                programBuilder.AppendFormat("\t# {0}", expr.ToString());
                programBuilder.AppendLine();
                return register;
            }
            else if (expr is PreDecrementExpression)
            {
                var postIncrement = expr as PreDecrementExpression;
                var register = registerFile.FirstUsedRegister();
                programBuilder.AppendFormat("subi {0}, {1}, 1", register, register);
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

                programBuilder.AppendFormat("{0}:	.word {1}",array.Pointer.ToString(),"1000");
                
                programBuilder.AppendLine();
                var listAddress = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("la {0}, {1}", listAddress, array.Pointer.ToString());
                programBuilder.AppendFormat("\t# Pone la dirección de la lista en {0}", listAddress);
                programBuilder.AppendLine();
                registerFile.FreeRegister(listAddress);
                var index = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("li {0}, {1}", index, array.Value);
                programBuilder.AppendFormat("\t# Agrega el indice a {0}",index);
                programBuilder.AppendLine();
                programBuilder.AppendFormat("add {0}, {1}, {2}", index, index, index);
                programBuilder.Append("\t# Dobla el índice");
                programBuilder.AppendLine();
                programBuilder.AppendFormat("add {0}, {1}, {2}", index, index, index);
                programBuilder.Append("\t# Dobla el índice nuevamente (por 4x)");
                programBuilder.AppendLine();
                var indexAddress = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("add {0}, {1}, {2}", indexAddress, index, listAddress);
                programBuilder.Append("\t# Combina los dos elementos de la dirección.");
                programBuilder.AppendLine();
                registerFile.FreeRegister(index);
                var valueAddress = registerFile.FirstAvailableRegister();
                programBuilder.AppendFormat("lw {0}, 0({1})", valueAddress , indexAddress);
                programBuilder.Append("\t# Toma el valor indexado en el arreglo.");
                programBuilder.AppendLine();
                registerFile.FreeRegister(indexAddress);
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
    }
}

