using Przychodnia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class VisitsListItemDTO
    {
        public DateTime VisitId { get; set; }
        public VisitStatus VisitStatus { get; set; }
        public string VisitType { get; set; }
        public string Doctor { get; set; }
        public string DoctorId { get; set; }
    }
}
