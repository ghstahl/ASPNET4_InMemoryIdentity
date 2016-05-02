using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.IdentityServerCore.Constants
{
    public static class P5_Constants
    {
        public const string @namespace = "p5:";
    }
    public static class ClaimTypes
    {
        public const string AccountGuid = P5_Constants.@namespace + "accountguid";
        public const string UserGuid = P5_Constants.@namespace + "userguid";
        public const string ClientRequestNameValueCollection = P5_Constants.@namespace + "crnvc";
    }
}
