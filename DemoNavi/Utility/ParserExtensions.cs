using GOLD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.Utility
{
    static class ParserExtensions
    {
        internal static object GetData(this Reduction reduction, int index)
        {
            return reduction.get_Data(index);
        }

    }
}
