using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Recompilers.MIPS32
{
    class RegisterFile
    {
        Dictionary<String, Boolean> registers = new Dictionary<string,bool>();
        Dictionary<String, Boolean> argumentRegisters = new Dictionary<string, bool>();

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

        public RegisterFile(string arguments)
        {
            argumentRegisters["$a0"] = false;
            argumentRegisters["$a1"] = false;
            argumentRegisters["$a2"] = false;
            argumentRegisters["$a3"] = false;
        }

        public string FirstAvailableArgument()
        {
            foreach (var arg in argumentRegisters)
            {
                if (arg.Value == false)
                {
                    registers[arg.Key] = true;
                    return arg.Key;
                }
            }
            throw new Exception("FirstAvailableArgument()()-> Sin argumentos Disponibles");
        }

        internal void FreeArgument(string argument)
        {
            argumentRegisters[argument] = false;
        }

        #endregion

    }
}
