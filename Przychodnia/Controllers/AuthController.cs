using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Przychodnia.Interfaces;
using Przychodnia.Transfer.Token;
using Przychodnia.Transfer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Produces(typeof(TokenDTO))]
        public async Task<IActionResult> Login(LoginUserCommand loginUserCommand)
        {

            var loginResult = await _userService.LoginUserAsync(loginUserCommand);


            if (loginResult.Success == false)
            {

                ModelState.AddModelError("errorMessage", loginResult.ErrorMessage);
                

                return BadRequest(ModelState);
            }

            return Ok(loginResult.Value);
        }
    }

}
