﻿using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace P5.IdentityServerCore.IdSrv
{
    public static class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Custom Grant Client",
                    ClientId = "custom_grant_client",
                    Enabled = true,
                    Flow = Flows.Custom,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256()),
                    },
                    AllowedCustomGrantTypes = new List<string>
                    {
                        "custom"
                    },
                    AllowedScopes = new List<string>
                    {
                        "read",
                        "write",
                    }
                },
                new Client
                {
                    ClientName = "CustomWebApi1 Application",
                    ClientId = "CustomWebApi1",
                    Enabled = true,
                    Flow = Flows.Custom,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedCustomGrantTypes = new List<string>
                    {
                        "custom"
                    },
                    AllowedScopes = new List<string>
                    {
                        "CustomWebApi1"
                    }
                },
                new Client
                {
                    ClientName = "Console Client Application",
                    ClientId = "ConsoleApplication",
                    Enabled = true,
                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "WebApi1"
                    }
                },
                new Client
                {
                    ClientName = "WebApi1 Application",
                    ClientId = "WebApi1",
                    Enabled = true,
                    Flow = Flows.Custom,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("4B79A70F-3919-435C-B46C-571068F7AF37".Sha256())
                    },
                    AllowedCustomGrantTypes = new List<string>
                    {
                        "act-as"
                    },
                    AllowedScopes = new List<string>
                    {
                        "WebApi2"
                    }
                },
                // no human involved
                new Client
                {
                    ClientName = "Silicon-only Client",
                    ClientId = "silicon",
                    Enabled = true,
                    Flow = Flows.ClientCredentials,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        "api1"
                    },
                    AccessTokenType = AccessTokenType.Reference
                },

                // human is involved
                new Client
                {
                    ClientName = "Silicon on behalf of Carbon Client",
                    ClientId = "carbon",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,

                    Flow = Flows.ResourceOwner,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("21B5F798-BE55-42BC-8AA8-0025B903DC3B".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        "api1"
                    }
                }
            };
        }
    }
}
