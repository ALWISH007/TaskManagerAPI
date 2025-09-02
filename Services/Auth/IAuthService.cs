namespace TaskManagerAPI.Services.Auth;

public interface IAuthService
{
    Task<string?> AuthenticateAsync(string username, string password);
}