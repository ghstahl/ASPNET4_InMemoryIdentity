using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace CustomClientCredentialHost.Areas.Admin.Models
{
    public class IDSScopePageRecord
    {
        public string PagingState { get; set; }
        public string CurrentPagingState { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<Scope> Scopes { get; set; }
    }
}