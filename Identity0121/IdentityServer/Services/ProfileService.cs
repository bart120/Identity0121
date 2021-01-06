using IdentityModel;
using IdentityServer.AspIdentity;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userMgr;

        public ProfileService(UserManager<User> userMgr)
        {
            _userMgr = userMgr;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            User user = await _userMgr.GetUserAsync(context.Subject);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            

            context.IssuedClaims.AddRange(await GetClaimsFromUserAsync(user));
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.PreferredUserName, $"{user.Firstname} {user.Lastname}")
            };

            if (!string.IsNullOrWhiteSpace(user.Lastname))
                claims.Add(new Claim("name", user.Lastname));

            IList<string> roles = await _userMgr.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
