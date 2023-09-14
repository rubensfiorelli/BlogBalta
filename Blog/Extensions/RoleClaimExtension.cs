using Blog.Models;
using System.Security.Claims;

namespace Blog.Api.Extensions
{
    public static class RoleClaimExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var newClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Email),
                new (ClaimTypes.NameIdentifier, user.Id.ToString())

            };
            newClaims.AddRange(user.Roles.Select(role =>
                new Claim(ClaimTypes.Role, role.Slug))
            );
            return newClaims;
        }
    }
}
