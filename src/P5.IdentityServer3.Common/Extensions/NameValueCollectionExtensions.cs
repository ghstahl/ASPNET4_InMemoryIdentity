using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace P5.IdentityServer3.Common.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static bool Validate(this NameValueCollection nvc, IList<string> againstList)
        {
            if (againstList.Any(item => nvc[(string) item] == null))
            {
                return false;
            }
            return true;
        }
    }
}