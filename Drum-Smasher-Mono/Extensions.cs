using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono
{
    public static class Extensions
    {
        /// <summary>
        /// Swaps b with a and returns new a
        /// </summary>
        public static T Swap<T>(this T a, ref T b)
        {
            ref T tempB = ref b;
            b = a;

            return tempB;
        }
    }
}
