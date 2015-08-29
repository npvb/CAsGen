using DemoNavi.Recompilers;
using DemoNavi.Recompilers.Basic;
using DemoNavi.Recompilers.MIPS32;
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

            //            string program = @"
            //                               
            //                                int main() {
            //                          
            //                                     if(n == true)
            //                                     {
            //                                        for(i=0;i<52;i++,j--,h++)
            //                                        {
            //                                            while(i<59)
            //                                            {
            //                                                switch(n)
            //                                                {
            //                                                    case true: return x;
            //                                                    case false: {return y;}
            //                                                    default: return h;
            //                                                }
            //                                                int c = a+b;
            //                                                int c = a&b;
            //                                                int c = a|b;
            //                                                int c = a&&b;
            //                                                int c = a||b;
            //                                                int c = a>>b;
            //                                                int c = a<<b;
            //                                                int c = a*b;
            //                                                int c = a/b;
            //                                                int c = a^b;
            //                                                c = a+=b;
            //                                            }
            //                                        }
            //                                     }
            //    
            //                                     return x;
            //                              }";
            /*            var parser = new DemoNavi.MyParser();
                        string program = @"
            

            int add(int a, int b)
            {
                int res;
                res = a + b;
                return res;
            }";


                       // program = @"int main() { a = 1 + 2 + 3 + 4 + 5 + 6; } ";

                        parser.Parse(program);
                        var programa = parser.Program;
                        Recompiler rec = new Mips32Recompiler();
                        Console.WriteLine(rec.Recompile(0, programa));
                        Console.ReadKey();*/
                     Loop();
                     Console.ReadKey();
                 }

                 static string Read()
                 {
                     Console.Write("1 ---- (Expression) o 2 ---- (Statement):   ");
                     if (Console.ReadLine() == "1")
                     {
                         return string.Format(@"int default_main() {{ {0} }} ", Console.ReadLine());
                     }
                     return string.Format(@"{0}", Console.ReadLine());
                 }

                 static string Eval(string program)
                 {

                     var parser = new DemoNavi.MyParser();
                     parser.Parse(program);
                     var programa = parser.Program;
                     Recompiler rec = new Mips32Recompiler();

                     return rec.Recompile(0, programa);
                 }

                 static void Print(string program)
                 {
                     Console.WriteLine(program);

                 }

                 static void Loop()
                 {
                     while (true)
                     {
                         Print(Eval(Read()));

                     }
        }
    }
}
               
