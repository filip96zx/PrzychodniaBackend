using Microsoft.AspNetCore.Identity;
using Przychodnia.Models;
using Przychodnia.Transfer.PagedList;
using Przychodnia.Transfer.Token;
using Przychodnia.Transfer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenis.Core;

namespace Przychodnia.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> GetUserByIdAsync(int id);
        Task<Result<int>> CreateUserAsync(CreateUserCommand command);
        Task<Result<int>> UpdateUserAsync(UpdateUserCommand command);
        Task<Result<TokenDTO>> LoginUserAsync(LoginUserCommand loginUserCommand);
        Task<Result<User>> DeleteUserAsync(int id);
        Task<Result<PagedListDTO<UserDTO>>> ListUserAsync(ListQuery query);
        Task<Result<PagedListDTO<UserDTO>>> ListUserFilteredAsync(FilteredListQuery query);
        Task<Result<int>> RegisterUserAsync(CreateUserCommand command);
        Task<Result<string>> ConfirmEmailAsync(int userId, string code);
        Task SignOutUserAsync();
        Task<Result<int>> SendEmailResetPasswordAsync(ForgotUserPasswordCommand command);
        Task<Result<string>> ResetPasswordAsync(ResetUserPasswordCommand command);
        Task<Result<IdentityResult>> CreateRoleAsync(CreateRoleCommand command);
        Task<Result<IdentityResult>> AddRoleToUserAsync(AddRoleCommand command);
        Task<Result<IdentityResult>> RemoveRoleFromUserAsync(AddRoleCommand command);
        Task<Result<IList<String>>> GetUserRolesAsync(string userId);
        Task<Result<List<String>>> GetRolesListAsync();
        Task<Result<List<string>>> GetSpecialisations();
        Task<Result<string>> DeleteSpecialisationAsync(CreateRoleCommand command);
        Task<Result<List<string>>> GetRemovableSpecialisations();

    }
}
