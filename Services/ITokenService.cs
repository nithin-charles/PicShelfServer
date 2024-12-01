using Microsoft.AspNetCore.Identity;

namespace PicShelfServer.Services
{
    public interface ITokenService
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
