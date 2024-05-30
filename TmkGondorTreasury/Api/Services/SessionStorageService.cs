using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Services
{
    public class SessionStorageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionStorageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto?> GetUser(string email)
        {
            var session = _httpContextAccessor?.HttpContext?.Session;
            var userJson = session?.GetString("User_" + email);
            if (string.IsNullOrEmpty(userJson))
            {
                Console.WriteLine("User not found in session.");
                return null;
            }
            var user = JsonConvert.DeserializeObject<UserDto>(userJson);
            Console.WriteLine($"Retrieved user: {userJson}");
            return await Task.FromResult(user);
        }

        public async Task SaveUser(UserDto user)
        {
            if (user == null || user.Email == null)
            {
                throw new ArgumentNullException(user == null ? nameof(user) : nameof(user.Email));
            }

            var session = _httpContextAccessor?.HttpContext?.Session;

            // Check if the session is not null
            if (session != null)
            {
                // Check if the user is already saved in the session storage
                var alreadySavedUser = session.GetString("User_" + user.Email);
                if (!string.IsNullOrEmpty(alreadySavedUser))
                {
                    // If the user is already saved, remove it
                    await RemoveUser(user.Email);
                }
                // Save the user in the session storage
                var userJson = JsonConvert.SerializeObject(user);
                session.SetString("User_" + user.Email, userJson);

                // Log the saved session
                Console.WriteLine($"Saved user: {userJson}");
            }
            await Task.CompletedTask;
        }

        public async Task RemoveUser(string email)
        {
            var session = _httpContextAccessor?.HttpContext?.Session;
            session?.Remove("User_" + email);
            await Task.CompletedTask;
        }

        public async Task Clear()
        {
            var session = _httpContextAccessor?.HttpContext?.Session;
            session?.Clear();
            await Task.CompletedTask;
        }
    }
}