using Przychodnia.Models;
using Przychodnia.Transfer.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Interfaces
{
    public interface ITokenService
    {
        TokenDTO CreateToken(User user, IList<string> role);
    }
}
