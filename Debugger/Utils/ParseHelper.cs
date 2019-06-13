using System;

namespace ModTools.Utils
{
    internal static class ParseHelper
    {
        public static bool TryParse(string text, Type targetType, out object result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = default;
                return false;
            }

            try
            {
                result = Convert.ChangeType(text, targetType);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static bool TryParse<T>(string text, out T result)
            where T : struct
        {
            if (string.IsNullOrEmpty(text))
            {
                result = default;
                return false;
            }

            try
            {
                return Parsers<T>.TryParse(text, out result);
            }
            catch
            {
                result = default;
                return false;
            }
        }

        private static class Parsers<T>
        {
            private static readonly TryParseDelegate Parser = (TryParseDelegate)Delegate.CreateDelegate(typeof(TryParseDelegate), typeof(T), "TryParse");

            private delegate bool TryParseDelegate(string text, out T result);

            public static bool TryParse(string text, out T result) => Parser(text, out result);
        }
    }
}
