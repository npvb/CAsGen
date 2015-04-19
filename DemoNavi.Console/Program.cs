﻿using DemoNavi.Recompilers;
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
            var parser = new DemoNavi.MyParser();
            string program = @"
                                struct st {
                                    int x;
                                    int y;
                                };
int x;
                                int main() {
                                y = x->as.gre[90];
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
            program = @"
int main()
{
    return add(10,20);
}

int add(int a, int b)
{
	int res;
	res = a + b;
	return res;
}";

            parser.Parse(program);
            var programa = parser.Program;
           Recompiler rec = new Mips32Recompiler();
           Console.WriteLine(rec.Recompile(programa));
            Console.ReadKey();
        }
    }
}
