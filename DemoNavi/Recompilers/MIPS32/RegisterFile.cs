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
        public RegisterFile()
        {
            registers["$t0"] = false;
            registers["$t1"] = false;
            registers["$t2"] = false;
            registers["$t3"] = false;
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
            throw new Exception("FirstAvailableRegister sin registro");
        }

        internal void FreeRegister(string register)
        {
            registers[register] = false;
        }

       
    }
}
