﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class GetVisitDetailsDTO
    {
        public DateTime VisitId { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
    }
}
