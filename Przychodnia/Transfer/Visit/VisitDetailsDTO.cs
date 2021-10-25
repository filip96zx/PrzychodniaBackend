using Przychodnia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class VisitDetailsDTO
    {
        public DateTime VisitId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string Doctor { get; set; }
        public string Patient { get; set; }
        public string VisitType { get; set; }
        public VisitStatus VisitStatus { get; set; }
        public List<string> Findings { get; set; }
        public List<string> Prescriptions { get; set; }
        public List<string> Messages { get; set; }
        public string DoctorEmail { get; set; }
        public string DoctorPhoneNumber { get; set; }
        public List<string> DoctorSpecialisations { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string PatientCountry { get; set; }
        public string PatientAddress { get; set; }
        public Gender PatientGender { get; set; }

    }
}
