using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class DeletePrescriptionDTO
    {
        public DateTime VisitId { get; set; }
        public int PrescriptionNumber { get; set; }
    }
}
