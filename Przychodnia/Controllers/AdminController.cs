using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przychodnia.Interfaces;
using Przychodnia.Transfer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;


        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("CreateSpecialisation")]
        public async Task<IActionResult> CreateSpecialisation(CreateRoleCommand command)
        {
            var result = await _userService.CreateRoleAsync(command);
            if (!result.Success)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);

                return BadRequest(ModelState);
            }

            return Ok();
        }
        [HttpPost("DeleteSpecialisation")]
        public async Task<IActionResult> DeleteSpecialisation(CreateRoleCommand command)
        {
            var result = await _userService.DeleteSpecialisationAsync(command);
            if (!result.Success)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);
                
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet("GetSpecialisations")]
        public async Task<IActionResult> GetSpecialisations()
        {
            var result = await _userService.GetSpecialisations();
            if (!result.Success)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);

                return BadRequest(ModelState);
            }
            return Ok(result);
        }

        [HttpGet("GetRemovableSpecialisations")]
        public async Task<IActionResult> GetRemovableSpecialisations()
        {
            var result = await _userService.GetRemovableSpecialisations();
            if (!result.Success)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);

                return BadRequest(ModelState);
            }
            return Ok(result);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(AddRoleCommand command)
        {
            var result = await _userService.AddRoleToUserAsync(command);
            if (!result.Success)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);

                return BadRequest(ModelState);
            }

            return Ok();
        }
        [HttpPost("RemoveRole")]
        public async Task<IActionResult> RemoveRole(AddRoleCommand command)
        {
            var result = await _userService.RemoveRoleFromUserAsync(command);
            if (!result.Success)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _userService.GetRolesListAsync();

            return Ok(result);
        }

        [HttpGet("UserRoles/{id}")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            
            var result = await _userService.GetUserRolesAsync(id);

            return Ok(result);
        }


    }
}
