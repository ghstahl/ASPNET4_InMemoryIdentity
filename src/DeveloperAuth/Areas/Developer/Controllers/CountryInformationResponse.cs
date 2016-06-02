using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P5.IdentityServer3.Admin.Areas.IdentityServer3Admin.Controllers
{
    class CountryInformationResponse
    {
        public string CountryCode { get; set; }
        public bool isEmbargoed { get; set; }
    }
}
