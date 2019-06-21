using UnityEngine;

namespace ModTools.Utils
{
    internal static class HashUtil
    {
        private static readonly long[] LargePrimes = new[]
        {
            8100529L,
            12474907L,
            15485039L,
            21768739L,
            28644467L,
            32452681L,
        };

        public static long HashRect(Rect rect)
        {
            long state = 18021301;
            Accumulate(ref state, rect.x, 0);
            Accumulate(ref state, rect.y, 0);
            Accumulate(ref state, rect.width, 0);
            Accumulate(ref state, rect.height, 0);
            return state;
        }

        public static string HashToString(long hash) => $"{hash:X}";

        private static void Accumulate(ref long state, float value, int index) => state ^= (long)value * LargePrimes[index];
    }
}