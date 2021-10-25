using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class GetVisitsDTO
    {
        public DateTime WeekDay { get; set; }
        public string DoctorId { get; set; }
        public string DoctorType { get; set; }
    }
}
