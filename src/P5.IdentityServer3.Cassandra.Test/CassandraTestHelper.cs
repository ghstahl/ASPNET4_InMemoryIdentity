﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Newtonsoft.Json;
 
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.RefreshToken;

namespace P5.IdentityServer3.Cassandra.Test
{
    public static class CassandraTestHelper
    {
        public static async Task<List<FlattenedScopeRecord>> InsertTestData_Scopes(int count = 1)
        {
            var result = new List<FlattenedScopeRecord>();
            for (int i = 0; i < count; ++i)
            {
                var name = "ScopeName:" + Guid.NewGuid();
                global::IdentityServer3.Core.Models.Scope record = new global::IdentityServer3.Core.Models.Scope()
                {
                    AllowUnrestrictedIntrospection = true,
                    Name = name,
                    ClaimsRule = "ClaimRule:" + i,
                    Description = "Description:" + i,
                    DisplayName = "DisplayName:" + i,
                    Enabled = true,
                    Emphasize = true,
                    IncludeAllClaimsForUser = true,
                    Required = true,
                    Type = ScopeType.Identity,
                    ScopeSecrets = new List<Secret>()
                    {
                        new Secret
                        {
                            Type = "Type1:" + i,
                            Description = "Decription1:" + i,
                            Expiration = DateTimeOffset.UtcNow,
                            Value = "Value1:" + i
                        },
                        new Secret
                        {
                            Type = "Type2:" + i,
                            Description = "Decription2:" + i,
                            Expiration = DateTimeOffset.UtcNow,
                            Value = "Value2:" + i
                        }
                    },
                    ShowInDiscoveryDocument = true,
                    Claims = new List<ScopeClaim>()
                    {
                        new ScopeClaim
                        {
                            AlwaysIncludeInIdToken = true,
                            Description = "Decription1:" + i,
                            Name = "Name1:" + i
                        },
                        new ScopeClaim
                        {
                            AlwaysIncludeInIdToken = true,
                            Description = "Decription2:" + i,
                            Name = "Name2:" + i
                        }
                    }
                };
                var scopeRecord = new FlattenedScopeRecord(new FlattenedScopeHandle(record));
                result.Add(scopeRecord);
            }
            await IdentityServer3CassandraDao.UpsertManyScopeAsync(result);
            return result;
        }

        public static async Task<List<FlattenedClientRecord>> InsertTestData_Clients(int count = 1)
        {
            List<FlattenedClientRecord> result = new List<FlattenedClientRecord>();
            for (int i = 0; i < count; ++i)
            {
                var name = "ClientName:" + Guid.NewGuid();
                global::IdentityServer3.Core.Models.Client client = new global::IdentityServer3.Core.Models.Client()
                {
                    AbsoluteRefreshTokenLifetime = 1,
                    AccessTokenLifetime = 1,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessToAllScopes = true,
                    AllowAccessToAllCustomGrantTypes = true,
                    AllowClientCredentialsOnly = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowRememberConsent = true,
                    AlwaysSendClientClaims = true,
                    AuthorizationCodeLifetime = 1,
                    AllowedCorsOrigins = new List<string>()
                    {
                        "AllowedCorsOrigins 1:" + i,
                        "AllowedCorsOrigins 2:" + i
                    },
                    AllowedCustomGrantTypes = new List<string>()
                    {
                        "AllowedCustomGrantTypes 1:" + i,
                        "AllowedCustomGrantTypes 2:" + i
                    },
                    AllowedScopes = new List<string>()
                    {
                        "AllowedScopes 1:" + i,
                        "AllowedScopes 2:" + i
                    },
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = name,
                    ClientUri = "ClientUri:" + i,
                    Claims = new List<Claim>()
                    {
                        new Claim("Type:" + i, "Value:" + i, "ValueType:" + i, "Issuer:" + i, "OriginalIssuer:" + i,
                            new ClaimsIdentity(new List<Claim>() {new Claim("Type:" + i, "Value:" + i)}))

                    },
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("Value:" + i, "Description:" + i, DateTimeOffset.UtcNow)
                    },
                    EnableLocalLogin = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    Enabled = true,
                    Flow = Flows.AuthorizationCode,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    IncludeJwtId = true,
                    LogoutSessionRequired = true,
                    SlidingRefreshTokenLifetime = 1,
                    IdentityTokenLifetime = 1,
                    LogoUri = "LogoUri:" + i,
                    LogoutUri = "LogoutUri:" + i,
                    PrefixClientClaims = true,
                    RequireConsent = true,
                    RequireSignOutPrompt = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    IdentityProviderRestrictions = new List<string>()
                    {
                        "IdentityProviderRestrictions 1:" + i,
                        "IdentityProviderRestrictions 2:" + i
                    },
                    RedirectUris = new List<string>()
                    {
                        "RedirectUris 1:" + i,
                        "RedirectUris 2:" + i
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "PostLogoutRedirectUris 1:" + i,
                        "PostLogoutRedirectUris 2:" + i
                    }
                };
                FlattenedClientRecord clientrecord = new FlattenedClientRecord(new FlattenedClientHandle(client));

                result.Add(clientrecord);
            }
            await IdentityServer3CassandraDao.UpsertManyClientAsync(result);
            return result;
        }

        public static async Task<List<FlattenedRefreshTokenHandle>> InsertTestData_RefreshTokens(IClientStore store, int count = 1)
        {
            var tokenInsert = InsertTestData_Tokens(count);
            var result = new List<FlattenedRefreshTokenHandle>();
            foreach (var token in tokenInsert.Result)
            {
                var accessToken = await token.MakeIdentityServerTokenAsync(store);
                var rt = new RefreshToken
                {
                    AccessToken = accessToken,
                    CreationTime = DateTimeOffset.UtcNow,
                    LifeTime = 5,
                    Version = 1
                };
                var rth = new FlattenedRefreshTokenHandle(token.Key,rt);
                result.Add(rth);
            }
            await IdentityServer3CassandraDao.CreateManyRefreshTokenHandleAsync(result);

            return result;
        }

        public static async Task<List<FlattenedAuthorizationCodeHandle>> InsertTestData_AuthorizationCode(int count = 1)
        {
            IClientStore clientStore = new ClientStore();
            var insertTokens = await CassandraTestHelper.InsertTestData_Tokens(count); // only add one client
            var clientId = insertTokens[0].ClientId;
            var clientRecord = await clientStore.FindClientByIdAsync(clientId);

            List<FlattenedAuthorizationCodeHandle> result = new List<FlattenedAuthorizationCodeHandle>();
            int i = 0;
            foreach (var insertToken in insertTokens)
            {
                var claimIdentityRecords = new List<ClaimIdentityRecord>()
                {
                    new ClaimIdentityRecord()
                    {
                        AuthenticationType = Constants.PrimaryAuthenticationType,
                        ClaimTypeRecords = new List<ClaimTypeRecord>()
                        {
                            new ClaimTypeRecord()
                            {
                                Type = Constants.ClaimTypes.Subject,
                                Value = "Value:" + i,
                                ValueType = "VALUETYPE:" + i
                            }
                        }
                    }
                };
                FlattenedAuthorizationCodeHandle handle = new FlattenedAuthorizationCodeHandle
                {
                    ClientId = clientId,
                    SubjectId = insertToken.SubjectId,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                    CreationTime = DateTimeOffset.UtcNow,
                    IsOpenId = true,
                    RedirectUri = "REDIRECTURI/" + i,
                    WasConsentShown = true,
                    Nonce = "NONCE:" + i,

                    ClaimIdentityRecords = new FlattenedAuthorizationCodeHandle().SerializeClaimsIdentityRecords(claimIdentityRecords),
                    RequestedScopes = new FlattenedAuthorizationCodeHandle().SerializeRequestScopes(clientRecord.AllowedScopes),
                    Key = Guid.NewGuid().ToString()
                };
                ++i;
                result.Add(handle);
            }
            await IdentityServer3CassandraDao.CreateManyAuthorizationCodeHandleAsync(result);
            return result;

        }

        public static async Task<List<FlattenedTokenHandle>> InsertTestData_Tokens(int count = 1)
        {
            var insertClients = await CassandraTestHelper.InsertTestData_Clients(1); // only add one client
            // we are going to associate a bunch of tokens to this one client

            var client = insertClients[0];
            var subjectId = Guid.NewGuid().ToString();
            List<FlattenedTokenHandle> result = new List<FlattenedTokenHandle>();
            for (int i = 0; i < count; ++i)
            {

                var claims = new List<Claim>()
                {
                    new Claim(Constants.ClaimTypes.Subject, subjectId),
                    new Claim(Constants.ClaimTypes.Name, "Name:" + i)
                };
                var json = JsonConvert.SerializeObject(claims);

                var flat = new FlattenedTokenHandle
                {
                    Key = Guid.NewGuid().ToString(),
                    Audience = "Audience:" + i,
                    Claims = JsonConvert.SerializeObject(claims),
                    ClientId = client.Record.ClientId,
                    CreationTime = DateTimeOffset.UtcNow,
                    Expires = DateTimeOffset.UtcNow,
                    Issuer = "Issuer:" + i,
                    Lifetime = 1,
                    SubjectId = subjectId,
                    Type = "Type:" + i,
                    Version = 1
                };

                result.Add(flat);
            }
            await IdentityServer3CassandraDao.CreateManyTokenHandleAsync(result);
            return result;
        }

        public static async Task<List<FlattenedConsentHandle>> InsertTestData_Consents(int count = 1)
        {
            var insertClients = await CassandraTestHelper.InsertTestData_Clients(1); // only add one client
            // we are going to associate a bunch of tokens to this one client

            var client = insertClients[0];
            var subject = Guid.NewGuid().ToString();
            return await InsertTestData_Consents(client.Record.ClientId,subject, count);
        }
        public static async Task<List<FlattenedConsentHandle>> InsertTestData_Consents(string clientId,string subject,int count = 1)
        {
            List<FlattenedConsentHandle> result = new List<FlattenedConsentHandle>();
            for (int i = 0; i < count; ++i)
            {
                var flat = new FlattenedConsentHandle(new Consent()
                {
                    ClientId = clientId,
                    Scopes = new List<string>()
                    {
                        "Scope 0:",
                        "Scope 1:"
                    },
                    Subject = subject
                });
                result.Add(flat);
            }
            await IdentityServer3CassandraDao.CreateManyConsentHandleAsync(result);
            return result;
        }
    }
}