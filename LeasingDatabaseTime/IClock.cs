using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeasingDatabaseTime
{
    /// <summary>
    /// Represents a clock that can tell the current time
    /// </summary>
    public interface IClock
    {
        DateTime Now { get; }
    }
}
