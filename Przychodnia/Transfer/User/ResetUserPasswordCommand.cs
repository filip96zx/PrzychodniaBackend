using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.User
{
    public class ResetUserPasswordCommand
    {
        public int UserId { get; set; }

        public string Code { get; set; }

        public string NewPassword { get; set; }

    }
}
