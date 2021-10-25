using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class RegisterToVisitDTO
    {
        public DateTime VisitId { get; set; }
        public string DoctorId { get; set; }
    }
}
