using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class GetAllDoctorsVisitsDTO
    {
        public DateTime WeekDay { get; set; }
        public string DoctorType { get; set; }
    }
}
