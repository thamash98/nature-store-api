using CeyloneNature.Application.Dtos;

namespace CeyloneNature.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request);
    Task<AuthResponseDto> LoginAsync(LoginRequest request);
    Task<UserDto> GetMeAsync(string email);
}
