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
        
        public RegisterFile()
        {
            registers["EAX"] = false;
            registers["AL"] = false;
            registers["BL"] = false;
            registers["CL"] = false;
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

    }
}
