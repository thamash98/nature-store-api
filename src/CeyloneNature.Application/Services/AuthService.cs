using CeyloneNature.Application.Common;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using CeyloneNature.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CeyloneNature.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Name, email and password are required.");

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new ValidationException("An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ValidationException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "customer");

        var token = _tokenService.CreateToken(user, "customer");
        return new AuthResponseDto
        {
            Token = token,
            User = new UserDto { Id = user.Id, Name = user.Name, Email = user.Email!, Role = "customer", LoyaltyPoints = user.LoyaltyPoints }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedException("Invalid email or password.");

        var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!check.Succeeded)
            throw new UnauthorizedException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.Contains("admin") ? "admin" : "customer";

        var token = _tokenService.CreateToken(user, role);
        return new AuthResponseDto
        {
            Token = token,
            User = new UserDto { Id = user.Id, Name = user.Name, Email = user.Email!, Role = role, LoyaltyPoints = user.LoyaltyPoints }
        };
    }

    public async Task<UserDto> GetMeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.Contains("admin") ? "admin" : "customer";

        return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email!, Role = role, LoyaltyPoints = user.LoyaltyPoints };
    }
}
