using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace MVCClient
{
    public class RoleClaimAction : ClaimAction
    {
        public RoleClaimAction()
           : base("role", ClaimValueTypes.String)
        {
        }


        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            
            var test = userData.GetProperty("role");
            foreach (var item in test.EnumerateArray())
            {
                var role = item.GetString();
                Claim claim = new Claim("role", role, ValueType, issuer);
                if (!identity.HasClaim(c => c.Subject == claim.Subject
                                         && c.Value == claim.Value))
                {
                    identity.AddClaim(claim);
                }
            }
        }
    }
}
