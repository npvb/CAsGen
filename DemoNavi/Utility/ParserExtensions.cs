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

        internal static void InsertNotNull<T>(this IList<T> source, int index, T item) where T : class
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            source.Insert(index, item);

        }
        internal static void AddNotNull<T>(this IList<T> source, T item) where T : class
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            source.Add(item);

        }

    }
}
