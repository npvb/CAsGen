using DemoNavi.Recompilers;
using DemoNavi.Recompilers.Basic;
using DemoNavi.Recompilers.MIPS32;
using DemoNavi.Recompilers.x86;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Loop();       
            Console.ReadKey(); 
        }

        #region REPL
        public static bool toMips { get; set; }

        public static bool toX86 { get; set; }

        static string Read()
        {
           Console.Write(">>   ");
           string command = Console.ReadLine();

           if (command == "#expr" || command == "#EXPR")
           {
             Console.WriteLine("#expr >>  ");
             return string.Format(@"int default_main() {{ {0} }} ", ReadInput());
           }
           else if (command == "#stmnt" || command == "#STMNT")
           {
               Console.WriteLine("#stmnt >>  ");
               return string.Format(@"{0}", ReadInput());
           }
           else
               return string.Format("Comando no reconocido. Solamente #expr o #stmnt son validos");
         }

        private static string ReadInput()
        {
          StringBuilder sb = new StringBuilder();
          string line;

          do
          {
              line = Console.ReadLine();
              if (line == "toMips")
              {
                  toMips = true;
              }
              else if (line == "toX86" || line == "tox86")
              {
                  toX86 = true;
              }
              if (line != "toMips" && line != "toX86" )
              {
                  sb.Append(line);
              }
          } while (line != "toMips" && line != "toX86");  
          
            return sb.ToString();
         }

        static string Eval(string program)
        {
            Recompiler rec = null;
            var parser = new DemoNavi.MyParser();
            parser.Parse(program);
            var programa = parser.Program;

            if (toMips == true)
            {
                rec = new Mips32Recompiler();
            }
            else if (toX86 == true)
            {
                rec = new x86Recompiler();
            }
           // Recompiler rec = new Mips32Recompiler();
            return rec.Recompile(programa); 
         }

        static void Print(string program)
        {
           Console.WriteLine();
           Console.WriteLine(program);

        }

        static void Loop()
        {
            try
            {
                while (true)
                {
                    Print(Eval(Read()));

                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                Loop();
            }
        }

        #endregion 
    
    }
}

