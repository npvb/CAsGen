﻿using DemoNavi.IntermediateRepresentation;
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
        private void WriteFunction(FunctionDeclaration function, StringBuilder programBuilder)
        {
            programBuilder.AppendFormat("{0}:", GenerateLabel(function));
            programBuilder.AppendLine();
            //reservar espacio en la pila
            WriteBlock(function.Block, programBuilder);
            //restablecer la pila
        }
        private void WriteExpr(Expression expr, StringBuilder programBuilder) 
        {
            if (expr is DecValue) 
            {
                programBuilder.AppendFormat("addi $t0, $zero, {0}:", expr.ToString());
                programBuilder.AppendLine();
            }
            else if(expr is FunctionCallExpression)
            {
                WriteFunctionCallExp(expr as FunctionCallExpression,programBuilder);
            }
        }   
        private void WriteFunctionCallExp(FunctionCallExpression functioncall, StringBuilder programBuilder)
        {
            for (int i = 0; i < functioncall.Parameters.Exprlist.Count; i++)
            {
                programBuilder.AppendFormat("add $a0,$a0, {0}", GetTypeSizeInBytes(functioncall.FunctionDeclaration.Parameters[i].Type));
                programBuilder.AppendLine();
                WriteExpr(functioncall.Parameters.Exprlist[i], programBuilder);
                programBuilder.AppendLine("sw $t0,($a0)");
            }
            
        }
        private void WriteReturnStatement(ReturnStatement returnStatement, StringBuilder programBuilder) 
        {
            WriteExpr(returnStatement.ReturnExpression, programBuilder);
            programBuilder.AppendLine("add $v0, $t0, $zero");

        }
        private void WriteBlock(BlockStatement blockstatement, StringBuilder programBuilder) 
        {
            foreach (Statement statement in blockstatement.StatementList) 
            {
                if(statement is ReturnStatement)
                {
                    WriteReturnStatement(statement as ReturnStatement, programBuilder);
                }
            }
        }

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
            programBuilder.AppendLine("#todo"); 
        }

        public override string Recompile(Program program)
        {
            StringBuilder programBuilder = new StringBuilder();
            WriteDataHeader(program, programBuilder);
            WriteGlobalVariables(program, programBuilder);
            WriteTextHeader(program, programBuilder);
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
