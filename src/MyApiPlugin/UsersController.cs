using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace MyApiPlugin
{
    public class Address
    {
        public string street { get; set; }
        public string suite { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public Geo geo { get; set; }
    }
    public class Company
    {
        public string name { get; set; }
        public string catchPhrase { get; set; }
        public string bs { get; set; }
    }
    public class Geo
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public Address address { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public Company company { get; set; }
    }

    [RoutePrefix("api/v1/myplugin")]
    public class UsersController : ApiController
    {
        [Route("users")]
        [HttpGet]
        public async Task<IList<User>> GetUsersAsync()
        {
            var accept = "application/json";
            var uri = "http://jsonplaceholder.typicode.com/users";
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Accept = accept;
            var content = new MemoryStream();
            List<User> users;
            using (WebResponse response = await req.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {

                    // Read the bytes in responseStream and copy them to content.
                    await responseStream.CopyToAsync(content);
                    string result = Encoding.UTF8.GetString(content.ToArray());
                    users = JsonConvert.DeserializeObject<List<User>>(result);
                }
            }
            return users;
        }
    }
}