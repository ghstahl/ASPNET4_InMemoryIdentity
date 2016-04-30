using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;

namespace P5.IdentityServer3AllInOne.App.Config
{
    class ActAsGrantValidator : ICustomGrantValidator
    {
        private TokenValidator _validator;

        public ActAsGrantValidator(TokenValidator validator)
        {
            _validator = validator;
        }

        Task<CustomGrantValidationResult> ICustomGrantValidator.ValidateAsync(ValidatedTokenRequest request)
        {
            CustomGrantValidationResult grantResult = null;

            var param = request.Raw.Get("token");
            if (string.IsNullOrWhiteSpace(param))
            {
                grantResult = new CustomGrantValidationResult(Constants.TokenErrors.InvalidRequest);
            }

            var result = _validator.ValidateAccessTokenAsync(param).Result;
            if (result.IsError)
            {
                grantResult = new CustomGrantValidationResult(result.Error);
            }

            var subjectClaim = result.Claims.FirstOrDefault(x => x.Type == "sub");
            if (subjectClaim == null)
            {
                grantResult = new CustomGrantValidationResult(Constants.TokenErrors.InvalidRequest);
            }

            if (grantResult == null)
            {
                var subject = subjectClaim.Value;

                grantResult = new CustomGrantValidationResult(subject, "access_token", new Claim[]
                {
                    new Claim(P5.IdentityServerCore.Constants.ClaimTypes.AccountGuid, Guid.NewGuid().ToString()),
                });
            }

            return Task.FromResult(grantResult);
        }

        public string GrantType
        {
            get { return "act-as"; }
        }
    }

}