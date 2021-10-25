using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class SendMessageDTO
    {
        public string Message { get; set; }
        public DateTime VisitId { get; set; }

        public string DoctorId { get; set; }

    }
}
