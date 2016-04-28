using System.Configuration;

namespace P5.IdentityServer3.BiggyJson
{
    public class StoreSettings
    {
        public string Folder { get; set; }
        public string Database { get; set; }
        public string ClientCollection { get; set; }
        public string ScopeCollection { get; set; }
        public string ConsentCollection { get; set; }
        public string AuthorizationCodeCollection { get; set; }
        public string ConnectionString { get; set; }
        public string RefreshTokenCollection { get; set; }
        public string TokenHandleCollection { get; set; }
        public static StoreSettings UsingFolder(string folder)
        {
            var settings = DefaultSettings;
            settings.Folder = folder;
            return settings;
        }

        public static StoreSettings DefaultSettings
        {
            get
            {
                return new StoreSettings
                {
                    Database = "identityserver",
                    ClientCollection = "clients",
                    ScopeCollection = "scopes",
                    ConsentCollection = "consents",
                    AuthorizationCodeCollection = "authorizationCodes",
                    RefreshTokenCollection = "refreshtokens",
                    TokenHandleCollection = "tokenhandles"
                };
            }
        }

    }
}
