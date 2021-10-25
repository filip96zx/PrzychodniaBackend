using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class SendPrescriptionDTO
    {
        public DateTime VisitId { get; set; }
        public string Prescription { get; set; }
    }
}
