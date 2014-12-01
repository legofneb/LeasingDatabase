using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeasingDatabaseTime
{
    /// <summary>
    /// An IClock implementation that represents the current system time
    /// </summary>
    public sealed class SystemClock : IClock
    {
        /// <summary>
        /// Singleton Instance of SystemClock
        /// </summary>
        public static readonly SystemClock Instance = new SystemClock();

        /// <summary>
        /// Preventing external class instantiation
        /// </summary>
        private SystemClock()
        {
        }

        /// <summary>
        /// Gets current time as Instant
        /// </summary>
        public DateTime Now { get { return DateTime.Now; } }


    }
}
