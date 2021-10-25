using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Przychodnia.Interfaces;
using Przychodnia.Transfer.Token;
using Przychodnia.Models;

namespace Przychodnia.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public TokenDTO CreateToken(User user, IList<string> role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var lifeTime = Int16.Parse(_config["Jwt:ExpiresMinutes"]);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims: GetTokenUserClaims(user, role),
                expires: DateTime.Now.AddMinutes(lifeTime),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenDTO()
            {
                Token = tokenString,
                UserName = user.UserName,
                UserFirstName = user.Name ?? string.Empty,
                UserSurname = user.Surname ?? string.Empty,
                Role = role,
                Expiration = token.ValidTo
            };
        }

        private static IEnumerable<Claim> GetTokenUserClaims(User user, IList<string>roles)
        {
            return new List<Claim>
            {
                new Claim("UserName", user.Email ?? string.Empty),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserFirstName", user.Name ?? string.Empty),
                new Claim("UserSurname", user.Surname ?? string.Empty),
                new Claim(ClaimTypes.Role, string.Join(",", roles))
            };
        }
    }

}
