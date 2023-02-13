namespace Cheevly.Extensions {
    public static class TimeExtensions {

        public static bool HasElapsed(this DateTime? time, TimeSpan timeSpan, bool utc = false) {
            if (time == null) {
                return true;
            }

            if (utc) {
                return DateTime.UtcNow > time.Value.Add(timeSpan);
            }

            return DateTime.Now > time.Value.Add(timeSpan);
        }

        public static bool HasElapsed(this DateTime time, TimeSpan timeSpan, bool utc = false) {
            if (utc) {
                return DateTime.UtcNow > time.Add(timeSpan);
            }

            return DateTime.Now > time.Add(timeSpan);
        }

        public static DateTime FirstOfNextMonth(this DateTime source) {
            return new DateTime(source.Year, source.Month, source.Day).AddMonths(1).AddTicks(-1);
        }

        public static string ToIso(this DateTime date) {
            return date.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
        }

        public static long ToUnixTimestamp(this DateTime date) {
            return Convert.ToInt64(date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        public static DateTime FromUnixTimestamp(this long date) {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(date);
        }

        public static long ToJavascriptDate(this DateTime date) {
            return (date - new DateTime(1970, 1, 1)).Ticks / 10000;
        }

        public static DateTime FromJavascriptDate(this long date) {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(date * 10000)).ToLocalTime();
        }

        public static IEnumerable<DateTime> EachIntervalUntil(this DateTime from, DateTime thru, TimeSpan interval) {
            for (var day = from.Date; day.Date < thru.Date; day = day.Add(interval))
                yield return day;
        }

        public static IEnumerable<DateTime> EachDay(this DateTime startDate, DateTime endDate) {
            if (endDate <= startDate) {
                yield return endDate;
            }
            else {
                for (var day = startDate.Date; day.Date <= endDate.Date; day = day.AddDays(1)) {
                    yield return day;
                }
            }
        }

        public static TimeSpan Until(this DateTime from, int month, int day, string time) {
            var next = DateTime.Parse(string.Format("{0}/{1}/{2} {3}", month, day, from.Year, time));

            if (next < from) {
                next = next.AddYears(1);
            }

            return next - from;
        }

        public static TimeSpan Until(this DateTime from, int date, string time) {
            var next = DateTime.Parse(from.ToString());

            // Todo:  What happens if the month contains fewer days than the given date?
            while (next.Day != date) {
                next = next.AddDays(1);
            }

            next = DateTime.Parse(string.Format("{0}/{1}/{2} {3}", next.Month, next.Day, next.Year, time));

            if (next < from) {
                next = next.AddDays(7);
            }

            return next - from;
        }

        public static TimeSpan Until(this DateTime from, DayOfWeek day, string time) {
            var next = DateTime.Parse(from.ToString());

            while (next.DayOfWeek != day) {
                next = next.AddDays(1);
            }

            next = DateTime.Parse(string.Format("{0}/{1}/{2} {3}", next.Month, next.Day, next.Year, time));

            if (next < from) {
                next = next.AddDays(7);
            }

            return next - from;
        }

        public static TimeSpan Until(this DateTime from, string time) {
            var date = DateTime.Parse(string.Format("{0}/{1}/{2} {3}", from.Month, from.Day, from.Year, time));

            if (date < from) {
                date = date.AddDays(1);
            }

            return date - from;
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek) {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0) {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
    }
}