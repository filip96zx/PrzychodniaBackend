using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Przychodnia.Interfaces;
using Przychodnia.Models;
using Przychodnia.Npgsql;
using Przychodnia.Transfer.PagedList;
using Przychodnia.Transfer.Token;
using Przychodnia.Transfer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenis.Core;

namespace Przychodnia.Services
{
    public class UserService : IUserService
    {
        private UserManager<User> UserManager { get; set; }
        private RoleManager<Role> RoleManager { get; set; }
        private DatabaseContext DataContext { get; set; }
        private SignInManager<User> SigInManager { get; set; }
        private IEmailSender EmailSender { get; set; }
        private ITokenService TokenService { get; set; }
        private IConfiguration Config;

        public UserService(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            DatabaseContext dataContext,
            SignInManager<User> sigInManager,
            ITokenService tokenService,
            IEmailSender emailSender,
            IConfiguration config)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            DataContext = dataContext;
            SigInManager = sigInManager;
            TokenService = tokenService;
            EmailSender = emailSender;
            Config = config;
        }

        public async Task<Result<User>> GetUserByIdAsync(int id)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return Result.Error<User>($"User with that id {id} doesn't exist");
            }
            return Result.Ok(user);
        }

        public async Task<Result<int>> CreateUserAsync(CreateUserCommand command)
        {
            if (command == null)
            {
                return Result.Error<int>("Create user command can`t be null");
            }

            var user = new User()
            {
                Name = command.Name,
                Surname = command.Surname,
                DateOfBirth = command.DateOfBirth,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                Country = command.Country,
                City = command.City,
                Gender = command.Gender,
                Address = command.Address,
                UserName = command.UserName
            };
            var userResult = await UserManager.CreateAsync(user, command.Password);
            if (userResult.Succeeded)
            {
                var userTmp = await UserManager.FindByNameAsync(user.UserName);
                return Result.Ok(userTmp.Id);
            }
            else
            {
                return Result.Error<int>(identityErrors: userResult.Errors);
            }

        }



        public async Task<Result<int>> UpdateUserAsync(UpdateUserCommand command)
        {
            if (command == null)
            {
                return Result.Error<int>("Update user command can`t be null");
            }
            var user = await UserManager.FindByIdAsync(command.UserId);
            user.City = command.City;
            user.Country = command.Country;
            user.Gender = command.Gender;
            user.Name = command.Name;
            user.EmailConfirmed = command.IsConfirmed;
            user.Surname = command.Surname;
            var result = await UserManager.UpdateAsync(user);
            DataContext.SaveChanges();
            return result.Succeeded ? Result.Ok(user.Id) : Result.Error<int>(result.Errors);
        }

        public async Task<Result<TokenDTO>> LoginUserAsync(LoginUserCommand loginUserCommand)
        {
            var loginResult = await SigInManager.PasswordSignInAsync(loginUserCommand.Login, loginUserCommand.Password,
               false, false);

            if (loginResult.Succeeded == false)
            {
                return Result.Error<TokenDTO>($"Data is incorrect");
            }

            var userResult = UserManager.FindByNameAsync(loginUserCommand.Login);

            if (userResult == null)
            {
                return Result.Error<TokenDTO>($"Data is incorrect");
            }

            var roles = await UserManager.GetRolesAsync(userResult.Result);
            var token = TokenService.CreateToken(userResult.Result, roles);

            return Result.Ok(token);
        }

        public async Task<Result<User>> DeleteUserAsync(int id)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            var deleteResult = await UserManager.DeleteAsync(user);
            if (deleteResult.Succeeded == false)
            {
                return Result.Error<User>(deleteResult.Errors);
            }

            return Result.Ok(user);
        }

        public async Task<Result<PagedListDTO<UserDTO>>> ListUserAsync(ListQuery query)
        {
            var viewModel = new PagedListDTO<UserDTO>()
            {
                PageSize = query.PageSize,
                PageIndex = query.PageIndex,
                TotalCount = DataContext.Users.Count()
            };
            var users = await DataContext.Users.Select(x =>

                new UserDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                })
                .OrderBy(user => user.Surname)
                .Skip(query.PageSize * query.PageIndex)
                .Take(query.PageSize)
                .ToListAsync();

            foreach (UserDTO user in users){
                var usr = await UserManager.FindByIdAsync(user.Id.ToString());
                var roleList = await UserManager.GetRolesAsync(usr);
                user.Roles = roleList;
            }
            viewModel.Item = users.OrderBy(user => user.Surname);
            return Result.Ok(viewModel);
        }

        public async Task<Result<PagedListDTO<UserDTO>>> ListUserFilteredAsync(FilteredListQuery query)
        {
            string searchLower = query.Search?.ToLower();

            if (searchLower == "") return Result.Error<PagedListDTO<UserDTO>>("Wrong Data.");

            var user = DataContext.Users.Where(user => user.Name.ToLower().Contains(searchLower) || user.Surname.ToLower().Contains(searchLower) || user.Email.ToLower().Contains(searchLower)
                                                || user.Address.ToLower().Contains(searchLower) || user.City.ToLower().Contains(searchLower) || user.PhoneNumber.ToLower().Contains(searchLower) ||
                                                user.Country.ToLower().Contains(searchLower));
            var userInRole = await UserManager.GetUsersInRoleAsync(query.Search);
            var users = user.Select(x =>

                new UserDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                }).ToList();

            foreach (User singleUserInRole in userInRole)
            {
                users.Add(new UserDTO
                {
                    Id = singleUserInRole.Id,
                    Name = singleUserInRole.Name,
                    Surname = singleUserInRole.Surname,
                    Email = singleUserInRole.Email,
                    PhoneNumber = singleUserInRole.PhoneNumber,
                });
            }


            var newlist = users.Distinct();
            
            var resultList = newlist.OrderBy(user => user.Surname).Skip(query.PageSize * query.PageIndex)
                .Take(query.PageSize)
                .ToList();

            var viewModel = new PagedListDTO<UserDTO>()
            {
                PageSize = query.PageSize,
                PageIndex = query.PageIndex,
                TotalCount = newlist.Count()
            };

            foreach (UserDTO singleUser in resultList)
            {
                var usr = await UserManager.FindByIdAsync(singleUser.Id.ToString());
                var roleList = await UserManager.GetRolesAsync(usr);
                singleUser.Roles = roleList;
            }
            viewModel.Item = resultList;
            return Result.Ok(viewModel);
        }

        public async Task<Result<int>> RegisterUserAsync(CreateUserCommand command)
        {
            var createUserResult = await CreateUserAsync(command);
            if (!createUserResult.Success)
            {
                return createUserResult;
            }
            var user = await UserManager.FindByIdAsync(createUserResult.Value.ToString());
            await UserManager.AddToRoleAsync(user, "user");
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

            var url = $"{Config["Email:ConfirmEmail"]}/{createUserResult.Value}/{codeEncoded}";
            var bodyBuilder = new BodyBuilder { HtmlBody = $"<html>Aby potwierdzić rejestracje <a href={url}>kliknij tu!</a>.</html>" };
            var message = new MimeMessage()
            {
                Body = bodyBuilder.ToMessageBody()
            };
            await EmailSender.SendEmailAsync(command.Email, "Potwierdzenie rejestracji", message);

            return Result.Ok(createUserResult.Value);
        }

        public async Task<Result<string>> ConfirmEmailAsync(int userId, string code)
        {
            var userResult = await GetUserByIdAsync(userId);
            if (!userResult.Success)
            {
                return Result.Error<string>(userResult.ErrorMessage);
            }
            var codeDecodedBytes = WebEncoders.Base64UrlDecode(code);
            var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);

            var result = await UserManager.ConfirmEmailAsync(userResult.Value, codeDecoded);
            if (!result.Succeeded)
            {
                return Result.Error<string>(result.Errors);
            }
            return Result.Ok<string>(null);
        }

        public async Task SignOutUserAsync()
        {
            await SigInManager.SignOutAsync();
        }

        public async Task<Result<int>> SendEmailResetPasswordAsync(ForgotUserPasswordCommand command)
        {
            if (command.UserName == null)
            {
                return Result.Error<int>("user name is null");
            }

            var user = await UserManager.FindByNameAsync(command.UserName);

            if ( user?.UserName.Length > 0 ) {
                var token = await UserManager.GeneratePasswordResetTokenAsync(user);
                var tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
                var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
#if DEBUG
                var url = $"{Config["Email:ResetPassword"]}/{user.Id}/{codeEncoded}";
#endif
#if !DEBUG
var url = $"{Config["Email:ResetPasswordProd"]}/{user.Id}/{codeEncoded}";
#endif
                var bodyBuilder = new BodyBuilder { HtmlBody = $"<html>Aby zmienić hasło <a href={url}>kliknij tu!</a>.</html>" };
                var message = new MimeMessage()
                {
                    Body = bodyBuilder.ToMessageBody()
                };
                await EmailSender.SendEmailAsync(user.Email, "Zmiana hasła", message);
                return Result.Ok(user.Id);
            } else return Result.Error<int>("User not exist");

        }

        public async Task<Result<string>> ResetPasswordAsync(ResetUserPasswordCommand command)
        {
            var userResult = await GetUserByIdAsync(command.UserId);
            if (!userResult.Success)
            {
                return Result.Error<string>(userResult.ErrorMessage);
            }
            var codeDecodedBytes = WebEncoders.Base64UrlDecode(command.Code);
            var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);

            var result = await UserManager.ResetPasswordAsync(userResult.Value, codeDecoded, command.NewPassword);
            if (!result.Succeeded)
            {
                return Result.Error<string>(result.Errors);
            }

            return Result.Ok<string>(null);
        }

        public async Task<Result<IdentityResult>> CreateRoleAsync(CreateRoleCommand command)
        {
            Role role = new Role
            {
                Name = command.RoleName
            };

            var result = await RoleManager.CreateAsync(role);

            return Result.Ok(result);
        }


        public async Task<Result<IdentityResult>> AddRoleToUserAsync(AddRoleCommand command)
        {
            if (command.UserId == "" || command.RoleName == "") return Result.Error<IdentityResult>("Wrong data");
            var user = await UserManager.FindByIdAsync(command.UserId);

            var result = await UserManager.AddToRoleAsync(user, command.RoleName);

            return Result.Ok(result);

        }

        public async Task<Result<IdentityResult>> RemoveRoleFromUserAsync(AddRoleCommand command)
        {
            if (command.UserId == "" || command.RoleName == "") return Result.Error<IdentityResult>("Wrong data");
            var user = await UserManager.FindByIdAsync(command.UserId);

            var result = await UserManager.RemoveFromRoleAsync(user, command.RoleName);
            return Result.Ok(result);
        }

        public async Task<Result<IList<string>>> GetUserRolesAsync(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Error<IList<string>>($"User not found");
            }
            var roleList = await UserManager.GetRolesAsync(user);


            return Result.Ok(roleList);
        }

        public async Task<Result<List<string>>> GetRolesListAsync()
        {
            var roleList = await DataContext.Role.Select(x =>
                x.Name
            ).ToListAsync();
            if (roleList == null)
            {
                return Result.Error<List<string>>($"No roles");
            }
            roleList.Sort();
            return Result.Ok(roleList);
        }

        public async Task<Result<List<string>>> GetSpecialisations()
        {
            var rolesList = await DataContext.Role.Where(x => x.NormalizedName != "ADMIN" && x.NormalizedName != "USER" && x.NormalizedName != "DOCTOR").Select(x =>
                x.Name
            ).ToListAsync();
            rolesList.Sort();

            return Result.Ok(rolesList);
        }

        public async Task<Result<string>> DeleteSpecialisationAsync(CreateRoleCommand command)
        {
            Role role = new Role
            {
                Name = command.RoleName
            };

            var doctorsInSpecialisation = await UserManager.GetUsersInRoleAsync(command.RoleName);

            if (doctorsInSpecialisation.Count > 0) return Result.Error<string>("Can not delete this specialisation, some doctors are assigned to it");

            var roleToDel = DataContext.Role.Where(role => role.NormalizedName == command.RoleName.ToUpper()).FirstOrDefault();
            DataContext.Role.Remove(roleToDel);
            DataContext.SaveChanges();

            return Result.Ok(command.RoleName+" deleted");
        }

        public async Task<Result<List<string>>> GetRemovableSpecialisations()
        {
            var rolesList = await DataContext.Role.Where(x => x.NormalizedName != "ADMIN" && x.NormalizedName != "USER" && x.NormalizedName != "DOCTOR").Select(x =>
                x.Name
            ).ToListAsync();
            List<string> removableSpecialisations = new List<string>();

            foreach(string role in rolesList)
            {
                if (UserManager.GetUsersInRoleAsync(role).Result.Count == 0) removableSpecialisations.Add(role);
            }

            removableSpecialisations.Sort();

            return Result.Ok(removableSpecialisations);
        }

    }
}
