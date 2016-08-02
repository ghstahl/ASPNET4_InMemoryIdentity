using System;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public class UserDoesNotExitException : Exception
    {
        public UserDoesNotExitException():base("User does not exist!")
        {
 
        }
    }
}