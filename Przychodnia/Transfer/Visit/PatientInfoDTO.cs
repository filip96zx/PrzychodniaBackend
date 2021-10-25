using Przychodnia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class PatientInfoDTO
    {
      
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
    }
}
