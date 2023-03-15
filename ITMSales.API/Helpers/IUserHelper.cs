using System;
using ITMSales.Shared.Entities;
using ITMSales.Shared.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ITMSales.API.Helpers
{
	public interface IUserHelper
	{
        Task<User> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(LoginDTO model);

        Task LogoutAsync();
    }
}

