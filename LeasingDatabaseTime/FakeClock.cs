using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeasingDatabaseTime
{
    /// <summary>
    /// A Fakeable instance of IClock. Meant to be used ONLY in Testing.
    /// Meant to fake SystemClock.Instance.Now times
    /// </summary>
    public sealed class FakeClock : IClock
    {
        private DateTime now;

        public FakeClock(DateTime instant)
        {
            now = instant;
        }

        public DateTime Now
        {
            get { return now; }
        }
    }
}
