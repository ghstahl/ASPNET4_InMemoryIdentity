using System.Collections.Generic;

namespace DeveloperAuth.Models
{
    public class AutoPostModel
    {
        public string URI { get; set; }
        public Dictionary<string, string> FormDictionary = new Dictionary<string, string>();
    }
}