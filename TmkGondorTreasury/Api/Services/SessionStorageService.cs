using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TmkGondorTreasury.DTOs;
using System.Threading.Tasks;
namespace TmkGondorTreasury.Services;

public class SessionStorageService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// The function retrieves a user object from session storage based on the provided email address
    /// asynchronously.
    /// </summary>
    /// <param name="email">The `email` parameter is a string that represents the email address of the
    /// user whose information is being retrieved.</param>
    /// <returns>
    /// The method `GetUser` returns a `Task` that will eventually contain a `UserDto` object. If the
    /// `userJson` is empty or null, the method returns `null`. Otherwise, it deserializes the
    /// `userJson` into a `UserDto` object and returns it within a `Task`.
    /// </returns>
    public async Task<UserDto?> GetUser(string email)
    {
        var session = _httpContextAccessor?.HttpContext?.Session;
        var userJson = session?.GetString("User_" + email);
        if (string.IsNullOrEmpty(userJson))
        {
            return null;
        }
        var user = JsonConvert.DeserializeObject<UserDto>(userJson);
        return await Task.FromResult(user);
    }

    /// <summary>
    /// The SaveUser method saves a user object in the session storage after checking if the user is not
    /// null and if the user is already saved in the session.
    /// </summary>
    /// <param name="UserDto">UserDto is a data transfer object (DTO) that represents a user entity. It
    /// typically contains properties such as Id, Name, Email, and other relevant information about a
    /// user. In the provided code snippet, the SaveUser method takes a UserDto object as a parameter to
    /// save user information in the</param>
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
        }
        await Task.CompletedTask;
    }

    /// <summary>
    /// The RemoveUser function removes a user from the session based on their email address.
    /// </summary>
    /// <param name="email">The `email` parameter in the `RemoveUser` method is a string that represents
    /// the email address of the user whose data needs to be removed from the session.</param>
    public async Task RemoveUser(string email)
    {
        var session = _httpContextAccessor?.HttpContext?.Session;
        session?.Remove("User_" + email);
        await Task.CompletedTask;
    }

    /// <summary>
    /// The Clear function clears the session data in an asynchronous manner.
    /// </summary>
    public async Task Clear()
    {
        var session = _httpContextAccessor?.HttpContext?.Session;
        session?.Clear();
        await Task.CompletedTask;
    }


}