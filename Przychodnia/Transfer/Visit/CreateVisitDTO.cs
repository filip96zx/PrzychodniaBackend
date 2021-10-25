using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Visit
{
    public class CreateVisitListDTO
    {
        public List<CreateVisitDTO> VisitsList { get; set; }
    }
    public class CreateVisitDTO
    {
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; }

    }
}
