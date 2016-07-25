using System.Collections.Generic;
using IdentityServer3.Core.Models;
using P5.AspNet.Identity.Cassandra;

namespace CustomClientCredentialHost.Areas.Admin.Models
{
    public class AspNetIdentityPageRecord
    {
        public string PagingState { get; set; }
        public string CurrentPagingState { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<CassandraUser> Users { get; set; }
    }
}