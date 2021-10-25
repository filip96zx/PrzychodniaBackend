using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Core
{
    public enum VisitStatus
    {
        Free = 0,
        Reserved = 1,
        Done = 2,
        Cancelled = 3
    }
}
