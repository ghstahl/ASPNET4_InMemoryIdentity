using System.Collections.Generic;

namespace P5.IdentityServer3.Common.Providers
{
    public interface IOptionalParams
    {
        void SetOptionalParams(IDictionary<string, string> optionalParams);
    }
}