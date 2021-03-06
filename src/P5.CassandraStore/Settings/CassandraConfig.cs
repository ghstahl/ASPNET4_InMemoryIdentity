﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.CassandraStore.Settings
{
    /*
      "Cassandra": {
          "KeySpace": "videodb",
          "ContactPoints": [
              "10.211.54.10"
          ],
          "Credentials": {
              "UserName": "",
              "Password": ""
          }
      }
      */

    public class CassandraCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CassandraConfig
    {
        public const string WellKnown_SectionName = "Cassandra";
        public string KeySpace { get; set; }
        public List<string> ContactPoints { get; set; }
        public CassandraCredentials Credentials { get; set; }
    }
}
