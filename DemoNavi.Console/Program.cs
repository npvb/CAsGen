using DemoNavi.Recompilers;
using DemoNavi.Recompilers.Basic;
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
            var parser = new DemoNavi.MyParser();
            string program = @"int main() {
                                    main();
                                    main(1);            
                                    main(1,x,main());
                                    main(main());
                                    main(main(),main(),~main());

                                     if(n == true)
                                     {
                                        for(i=0;i<52;i++,j--,h++)
                                        {
                                            while(i<59)
                                            {
                                                switch(n)
                                                {
                                                    case true: return x;
                                                    case false: {return y;}
                                                    default: return h;
                                                }
                                                int c = a+b;
                                                int c = a&b;
                                                int c = a|b;
                                                int c = a&&b;
                                                int c = a||b;
                                                int c = a>>b;
                                                int c = a<<b;
                                                int c = a*b;
                                                int c = a/b;
                                                int c = a^b;
                                                c = a+=b;
                                            }
                                        }
                                     }
    
                                     return x;
                              }";
            parser.Parse(program);
            var programa = parser.Program;
           // BasicRecompiler rec = new BasicRecompiler();
           //rec.Recompile(programa);
            Console.ReadKey();
        }
    }
}
