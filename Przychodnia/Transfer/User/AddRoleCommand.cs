using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.User
{
    public class AddRoleCommand
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }
}
