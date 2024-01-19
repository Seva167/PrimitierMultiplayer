using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Bridging
{
    public static class Il2CppUtils
    {
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this List<T> list)
        {
            var clist = new Il2CppSystem.Collections.Generic.List<T>(list.Count);
            foreach (var item in list)
            {
                clist.Add(item);
            }
            return clist;
        }

        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this T[] array)
        {
            var clist = new Il2CppSystem.Collections.Generic.List<T>(array.Length);
            foreach (var item in array)
            {
                clist.Add(item);
            }
            return clist;
        }
    }
}
