using iic_odk_auth.Models;
using Microsoft.Extensions.Primitives;

namespace iic_odk_auth.Services.OdkService
{
    public interface IOdkService
    {
        Task AddToProjectAsync(User user);
        Task<int> CreateUserAsync(User user);
        Task<int> GetUserOdkIdAsync(User user);
        Task<StringValues> Login(User user);
        Task<User> GetCurrentUserAsync(string cookie);
    }
}