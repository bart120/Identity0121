using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Demo
{
    public static class DemoCode
    {
        public static IEnumerable<ApiScope> ApiScopes =>
           new List<ApiScope>
           {
                new ApiScope("api_li", "API Loire Industrie")
           };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "console_devis_forge",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("key_devis_forge".Sha256())
                    },

                    AllowedScopes = {"api_li"}
                },

                new Client
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
                        "api_li"
                    },
                    RedirectUris = { "https://oauth.pstmn.io/v1/callback", "https://localhost:44396/signin-oidc" },
                    RequirePkce = false
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
           new List<IdentityResource>
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
           };
    }

    public static class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "47 rue Berger",
                    city = "Paris"
                };

                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "987654321",
                        Username = "remi@google.fr",
                        Password = "bob"
                    },
                    new TestUser
                    {
                        SubjectId = "12346789",
                        Username = "vincent@google.fr",
                        Password = "bob"
                    }
                };
            }
        }
    }

}
