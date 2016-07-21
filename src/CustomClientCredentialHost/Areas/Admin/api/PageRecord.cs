using System.Collections.Generic;
using P5.AspNet.Identity.Cassandra;

namespace CustomClientCredentialHost.Areas.Admin.api
{
    public class PageRecord
    {
        public string PagingState { get; set; }
        public string CurrentPagingState { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<CassandraUser> Users { get; set; }
    }
}