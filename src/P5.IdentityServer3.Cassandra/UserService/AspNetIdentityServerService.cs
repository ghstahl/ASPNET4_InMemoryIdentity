﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using Microsoft.AspNet.Identity;
using P5.AspNet.Identity.Cassandra;

namespace P5.IdentityServer3.Cassandra.UserService
{
    public class ArbritraryUserService : UserServiceBase
    {
        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            if (!string.IsNullOrEmpty(context.UserName))
            {
                context.AuthenticateResult = new AuthenticateResult(context.UserName, context.UserName);
            }
        }
    }
    public class AspNetIdentityServerService: UserServiceBase
    {
        private CassandraUserStore _userStore;
        public AspNetIdentityServerService(CassandraUserStore userStore)
        {
            _userStore = userStore;
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            return base.IsActiveAsync(context);
        }

        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            return base.AuthenticateExternalAsync(context);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            return base.GetProfileDataAsync(context);
        }

        public override Task PostAuthenticateAsync(PostAuthenticationContext context)
        {
            return base.PostAuthenticateAsync(context);
        }

        public override Task PreAuthenticateAsync(PreAuthenticationContext context)
        {
            return base.PreAuthenticateAsync(context);
        }

        public override Task SignOutAsync(SignOutContext context)
        {
            return base.SignOutAsync(context);
        }

        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var user = await _userStore.FindByEmailAsync(context.UserName);
            if (user != null)
            {
                IPasswordHasher passwordHasher = new PasswordHasher();
                var hash = passwordHasher.HashPassword(password: context.Password);
                var result = passwordHasher.VerifyHashedPassword(user.PasswordHash, context.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    context.AuthenticateResult = new AuthenticateResult(user.Id.ToString(), context.UserName);
                }
            }
        }
    }
}
