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

        private static StoreSettings _defaultSettings;

        public static StoreSettings DefaultSettings
        {
            get
            {
                if (_defaultSettings == null)
                {
                    _defaultSettings = new StoreSettings
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
                return _defaultSettings;
            }
        }

    }
}
