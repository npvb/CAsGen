using DemoNavi.IntermediateRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Recompilers.MIPS32
{
    public class RegisterFile
    {
        public struct Stack
        {
            public string id;
            public string value;
        }

        public Stack stack1;
        Dictionary<String, Boolean> registers = new Dictionary<string,bool>();
        Dictionary<String, Boolean> argumentRegisters = new Dictionary<string, bool>();
        Dictionary<String, Boolean> savedValues= new Dictionary<string, bool>();
        public Dictionary<String, String> savedTemporals = new Dictionary<string, string>();
        public Dictionary<String, Stack> savedValuesList = new Dictionary<string, Stack>();
        public Dictionary<String, String> savedArguments = new Dictionary<string, string>();
        public List<String> listofFunctions = new List<string>();

        
        #region Registers
     
        public RegisterFile()
        {
            registers["$t0"] = false;
            registers["$t1"] = false;
            registers["$t2"] = false;
            registers["$t3"] = false;
            registers["$t4"] = false;
            registers["$t5"] = false;
            registers["$t6"] = false;
            registers["$t7"] = false;
            registers["$t8"] = false;
            registers["$t9"] = false;
            savedValues["$s0"] = false;
            savedValues["$s1"] = false;
            savedValues["$s2"] = false;
            savedValues["$s3"] = false;
            savedValues["$s4"] = false;
            savedValues["$s5"] = false;
            savedValues["$s6"] = false;
            savedValues["$s7"] = false;
            argumentRegisters["$a0"] = false;
            argumentRegisters["$a1"] = false;
            argumentRegisters["$a2"] = false;
            argumentRegisters["$a3"] = false;
            //savedValuesList.Add(null,null);
            
        }

        public string FirstAvailableRegister()
        {
            foreach (var reg in registers)
            {
                if(reg.Value == false)
                {
                    registers[reg.Key] = true;
                    return reg.Key;
                }
            }
            throw new Exception("FirstAvailableRegister()-> Sin registro Temporales Disponibles");
        }

        public string FirstUsedRegister()
        {
            foreach (var reg in registers)
            {
                if (reg.Value == true)
                {
                    return reg.Key;
                }
            }
            throw new Exception("FirstUsedRegister()->Todos los Registros sin Usar");
        }

        internal void FreeRegister(string register)
        {
            registers[register] = false;
        }

        #endregion

        #region Arguments

        public string FirstAvailableArgument()
        {
            foreach (var arg in argumentRegisters)
            {
                if (arg.Value == false)
                {
                    argumentRegisters[arg.Key] = true;
                    return arg.Key;
                }
            }
            throw new Exception("FirstAvailableArgument()-> Sin argumentos Disponibles");
        }

        internal void FreeArgument(string argument)
        {
            argumentRegisters[argument] = false;
        }

        internal void FreeAllArgument()
        {
            argumentRegisters["$a0"] = false;
            argumentRegisters["$a1"] = false;
            argumentRegisters["$a2"] = false;
            argumentRegisters["$a3"] = false;
        }
        #endregion

        #region SavedValues

       /* public RegisterFile(bool savedValue)
        {
            savedValues["$s0"] = false;
            savedValues["$s1"] = false;
            savedValues["$s2"] = false;
            savedValues["$s3"] = false;
            savedValues["$s4"] = false;
            savedValues["$s5"] = false;
            savedValues["$s6"] = false;
            savedValues["$s7"] = false;
        }*/

        public string FirstAvailableSavedValue()
        {
            foreach (var sv in savedValues)
            {
                if (sv.Value == false)
                {
                    savedValues[sv.Key] = true;
                    return sv.Key;
                }
            }
            throw new Exception("FirstAvailableSavedValue()-> Sin valores en la pila disponibles");
        }

        internal void FreeSavedValue(string argument)
        {
            savedValues[argument] = false;
        }

        
        #endregion

  
    }
}
