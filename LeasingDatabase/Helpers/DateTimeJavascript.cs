using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Helpers
{
    public static class DateTimeJavascript
    {
        private static readonly long DateTimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static long ToJavaScriptMilliseconds(this DateTime dt)
        {
            return (long)((dt.ToUniversalTime().Ticks - DateTimeMinTimeTicks) / 10000);
        }

        public static DateTime ToDateTime(this long dt)
        {
            return new DateTime((dt * 10000) + DateTimeMinTimeTicks);
        }
    }
}