using CeyloneNature.Domain.Entities;

namespace CeyloneNature.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(ApplicationUser user, string role);
}
