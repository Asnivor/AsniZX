using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Common
{
    public static unsafe partial class Util
    {
        
        public static void Memset(void* ptr, int val, int len)
        {
            var bptr = (byte*)ptr;
            for (int i = 0; i < len; i++)
            {
                bptr[i] = (byte)val;
            }
        }
        
    }
}
