using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser([FromQuery] ListQuery query)
        {
            var result = await _userService.ListUserAsync(query);
            return Ok(result.Value);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetUsersFiltered([FromQuery] ListQuery query, SearchString search)
        {

            var result = await _userService.ListUserFilteredAsync(new FilteredListQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Search = search.Search
            });
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (result.Success == false)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);
                
                return BadRequest(ModelState);
            }

            var viewModel = new UpdateUserCommand()
            {
                UserId = result.Value.Id.ToString(),
                Name = result.Value.Name,
                Surname = result.Value.Surname,
                PhoneNumber = result.Value.PhoneNumber,
                DateOfBirth = result.Value.DateOfBirth,
                UserName = result.Value.UserName,
                IsConfirmed = result.Value.EmailConfirmed,
                Country = result.Value.Country,
                City = result.Value.City,
                Address = result.Value.Address,
                Gender = result.Value.Gender
            };
            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserCommand command)
        {
            var userIdResult = await _userService.CreateUserAsync(command);
            if (!userIdResult.Success)
            {

                    ModelState.AddModelError("errorMessage", userIdResult.ErrorMessage);
;
            }
            var userResult = await _userService.GetUserByIdAsync(userIdResult.Value);
            if (!userResult.Success)
            {

                ModelState.AddModelError("errorMessage", userResult.ErrorMessage);
                
                return BadRequest(ModelState);
            }
            var userDTO = new UserDTO()
            {
                Id = userResult.Value.Id,
                Name = userResult.Value.Name,
                Surname = userResult.Value.Surname,
                Email = userResult.Value.Email,
                PhoneNumber = userResult.Value.PhoneNumber
            };

            return CreatedAtAction(nameof(GetById),
                new { id = userDTO.Id }, userDTO);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserCommand command)
        {
            var userIdResult = await _userService.RegisterUserAsync(command);
            if (!userIdResult.Success)
            {

                ModelState.AddModelError("errorMessage", userIdResult.ErrorMessage);

                return BadRequest(ModelState);
            }

            return Ok();
        }


        [HttpPatch("UpdateUser")]
        public async Task<IActionResult> Update(UpdateUserCommand command)
        {

            var updateResult = await _userService.UpdateUserAsync(command);
            if (updateResult.Success == false)
            {


                ModelState.AddModelError("errorMessage", updateResult.ErrorMessage);

                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteResult = await _userService.DeleteUserAsync(id);
            if (deleteResult.Success == false)
            {

                ModelState.AddModelError("errorMessage", deleteResult.ErrorMessage);


                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [HttpGet("ConfirmEmail/{id}/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(int id, string code)
        {
            var result = await _userService.ConfirmEmailAsync(id, code);
            if (result.Success == false)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return Ok();

        }

        [HttpPost("Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOutUserAsync();
            return Ok();
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotUserPasswordCommand command)
        {
            var result = await _userService.SendEmailResetPasswordAsync(command);
            if (!result.Success)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);

                //return BadRequest(ModelState); 
                return Ok();
            }

            return Ok();
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [Produces(typeof(ResetUserPasswordCommand))]
        public async Task<IActionResult> ResetPassword(ResetUserPasswordCommand command)
        {
            var result = await _userService.ResetPasswordAsync(command);
            if (result.Success != true)
            {

                ModelState.AddModelError("errorMessage", result.ErrorMessage);
            
                return BadRequest(ModelState);
            } 
            return Ok();
        }


    }
}
