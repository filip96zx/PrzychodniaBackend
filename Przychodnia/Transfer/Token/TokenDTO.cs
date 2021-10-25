using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.Token
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserSurname { get; set; }
        public IList<string> Role { get; set; }
        public DateTime Expiration { get; set; }

    }
}
