using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;

namespace P5.IdentityServer3AllInOne.App.Config
{
    class CustomGrantValidator : ICustomGrantValidator
    {
        private IUserService _users;

        public CustomGrantValidator(IUserService users)
        {
            _users = users;
        }

        Task<CustomGrantValidationResult> ICustomGrantValidator.ValidateAsync(ValidatedTokenRequest request)
        {
            var param = request.Raw.Get("some_custom_parameter");
            if (string.IsNullOrWhiteSpace(param))
            {
                return Task.FromResult<CustomGrantValidationResult>(
                    new CustomGrantValidationResult("Missing parameters."));
            }
            var result = new CustomGrantValidationResult("bob", "customGrant");

            return Task.FromResult(result);
        }

        public string GrantType
        {
            get { return "custom"; }
        }
    }
}