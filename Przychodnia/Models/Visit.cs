using Przychodnia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Models
{
    public class Visit
    {
        public DateTime VisitId { get; set; }
        public int UserId { get; set; }
        public int PatientId { get; set; }
        public string VisitType { get; set; }
        public VisitStatus VisitStatus { get; set; }
        public List<string> Findings { get; set; }
        public List<string> Prescriptions { get; set; }
        public List<string> Messages { get; set; }
        public User User { get; set; }


    }
}
