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

    public async Task SaveUser(UserDto user)
    {
        var session = _httpContextAccessor?.HttpContext?.Session;
        if (session != null)
        {
            var userJson = JsonConvert.SerializeObject(user);
            session.SetString("User_" + user.Email, userJson);
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