using System;

namespace ModTools
{
    public static class DateTimeUtil
    {
        private const float secondsInMinute = 60.0f;
        private const float secondsInHour = secondsInMinute * 60.0f;
        private const float secondsInDay = secondsInHour * 24.0f;
        private const float secondsInWeek = secondsInDay * 7.0f;
        private const float secondsInYear = secondsInWeek * 52.0f;

        public static string TimeSpanToString(TimeSpan timeSpan)
        {
            double seconds = Math.Abs(timeSpan.TotalSeconds);
            if (seconds < secondsInMinute)
            {
                return "Less than a minute ago";
            }

            if (seconds < secondsInHour)
            {
                return TimeSpanToString((int)(seconds / secondsInMinute), "minute");
            }

            if (seconds < secondsInDay)
            {
                return TimeSpanToString((int)(seconds / secondsInHour), "hour");
            }

            if (seconds < secondsInWeek)
            {
                return TimeSpanToString((int)(seconds / secondsInDay), "day");
            }

            if (seconds < secondsInYear)
            {
                return TimeSpanToString((int)(seconds / secondsInWeek), "week");
            }

            return TimeSpanToString((int)(seconds / secondsInYear), "years");
        }

        private static string TimeSpanToString(int count, string s) => $"{count.ToString()} {Pluralize(s, count)} ago";

        public static string Pluralize(string s, int count)
        {
            if (count < 2)
            {
                return s;
            }

            return s + "s";
        }
    }
}