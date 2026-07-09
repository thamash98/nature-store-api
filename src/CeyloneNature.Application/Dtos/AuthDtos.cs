namespace CeyloneNature.Application.Dtos;

public class RegisterRequest
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "customer";
    public int LoyaltyPoints { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; } = "";
    public UserDto User { get; set; } = new();
}
