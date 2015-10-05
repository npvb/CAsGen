using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Recompilers.x86
{
    class RegisterFile
    {
        public struct Stack
        {
            public string idValue;
            public string positioninStack;
        }

        public Stack stackx86;
        public Dictionary<String, Stack> stack = new Dictionary<string, Stack>();
        Dictionary<String, Boolean> registers = new Dictionary<string, bool>();
        public Dictionary<String, Boolean> savedTemporals = new Dictionary<string, bool>();
        public List<String> listofFunctions = new List<string>();
        
        public RegisterFile()
        {
            registers["eax"] = false;
            registers["ebx"] = false;
            registers["ecx"] = false;
            registers["edx"] = false;
            registers["DL"] = false;
            registers["AH"] = false;
            registers["BH"] = false;
            registers["CH"] = false;
            registers["DH"] = false;
        }

        public string FirstAvailableRegister()
        {
            foreach (var reg in registers)
            {
                if (reg.Value == false)
                {
                    registers[reg.Key] = true;
                    return reg.Key;
                }
            }
            throw new Exception("FirstAvailableRegister()-> Sin registro Temporales Disponibles");
        }

        internal void FreeRegister(string register)
        {
            registers[register] = false;
        }

        internal string RegisterPosition(string id)
        {
            for (int x = 0; x < stack.Count(); x++ )
            {
                if (stack.ElementAt(x).Key == id) 
                {
                    return stack.ElementAt(x).Value.positioninStack;
                }
            }
                return "0";
            throw new Exception("FirstUsedRegister()->Todos los Registros sin Usar");
        }
    }
}
