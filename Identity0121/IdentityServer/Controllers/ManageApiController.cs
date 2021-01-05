using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.AspIdentity;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class ManageApiController : Controller
    {
        private readonly ConfigurationDbContext _confContext;
        private readonly UserManager<User> _userMgr;

        public ManageApiController(ConfigurationDbContext confContext, UserManager<User> userMgr)
        {
            _confContext = confContext;
            _userMgr = userMgr;
        }

        public async Task<IActionResult> AddApiResource()
        {
            IdentityServer4.Models.ApiResource api = new IdentityServer4.Models.ApiResource();
            api.Name = "api_li";
            api.Description = "une api en formation IdentityServer";
            api.DisplayName = "API Loire Industrie";
            api.Enabled = true;

            _confContext.ApiResources.Add(api.ToEntity());
            await _confContext.SaveChangesAsync();


            return Ok();
        }

        public async Task<IActionResult> AddApiScope()
        {
            IdentityServer4.Models.ApiScope apiScope = new IdentityServer4.Models.ApiScope();
            apiScope.Name = "api_li_scope";
            apiScope.Enabled = true;


            var scop = new IdentityResources.OpenId();
            var scop2 = new IdentityResources.Profile();

            _confContext.ApiScopes.Add(apiScope.ToEntity());
            _confContext.IdentityResources.Add(scop.ToEntity());
            _confContext.IdentityResources.Add(scop2.ToEntity());
            await _confContext.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> AddClient()
        {
            IdentityServer4.Models.Client client = new Client();
            client.ClientId = "console_devis_forge";
            client.ClientSecrets = new List<Secret> { new Secret("key_devis_forge".Sha256()) };
            client.AllowedScopes = new List<string> { "api_li_scope" };
            client.AllowedGrantTypes = GrantTypes.ClientCredentials;

            IdentityServer4.Models.Client client2 = new Client
            {
                ClientId = "mvc_devis_forge",
                ClientSecrets =
                    {
                        new Secret("key_devis_forge".Sha256())
                    },
                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api_li_scope"
                    },
                RedirectUris = { "https://oauth.pstmn.io/v1/callback", "https://localhost:44396/signin-oidc" },
                RequirePkce = false
            };

            _confContext.Clients.Add(client.ToEntity());
            _confContext.Clients.Add(client2.ToEntity());

            await _confContext.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> AddUser()
        {
            var u = new User();
            u.UserName = "remi@google.fr";
            u.Firstname = "Rémi";
            u.Lastname = "Berthe";
            u.Email = "remi@google.fr";
            u.EmailConfirmed = true;
            
            var result = _userMgr.CreateAsync(u, "Toto2021$").Result;
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            else
                return Ok();
        }

    }
}
