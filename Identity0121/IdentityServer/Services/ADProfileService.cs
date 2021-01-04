using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class ADProfileService : IProfileService
    {

        UserPrincipal userPrincipal = null;

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var adContext = new PrincipalContext(ContextType.Domain);

            var user = context.Subject.GetDisplayName();

            userPrincipal = UserPrincipal.FindByIdentity(adContext, IdentityType.SamAccountName, user);

            //standard claims
            var claims = new Claim[]
            {
                new Claim(JwtClaimTypes.Name, userPrincipal.Name),
                new Claim(JwtClaimTypes.GivenName, userPrincipal.GivenName),
                new Claim(JwtClaimTypes.FamilyName, userPrincipal.DisplayName),
                new Claim(JwtClaimTypes.Email, userPrincipal.EmailAddress),
                new Claim(JwtClaimTypes.Address, "Ici"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            };

            //context.AddRequestedClaims(claims);

            //personnal claims
            string department = "";
            if(userPrincipal.GetUnderlyingObjectType() == typeof(DirectoryEntry))
            {
                using(var entry = (DirectoryEntry)userPrincipal.GetUnderlyingObject())
                {
                    if(entry.Properties["department"] != null)
                    {
                        department = entry.Properties["department"].Value.ToString();
                    }
                }
            }


            List<Claim> cl = claims.ToList();
            cl.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/department", department));
            cl.Add(new Claim("claim_custom", userPrincipal.UserPrincipalName));

            context.IssuedClaims = cl;
            return Task.CompletedTask;

        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var adContext = new PrincipalContext(ContextType.Domain);

            var user = context.Subject;

            Claim userClaim = user.Claims.FirstOrDefault(cl => cl.Type == "sub ");

            userPrincipal = UserPrincipal.FindByIdentity(adContext, IdentityType.DistinguishedName, userClaim.Value);

            var isLocked = userPrincipal.IsAccountLockedOut();
            context.IsActive = (bool)(userPrincipal.Enabled & !isLocked);

            return Task.CompletedTask;
        }
    }
}
