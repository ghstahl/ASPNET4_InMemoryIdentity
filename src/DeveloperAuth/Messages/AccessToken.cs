namespace DeveloperAuth.Messages
{
    public class AccessToken : RequestToken
    {
        public string ScreenName { get; set; }

        public string UserId { get; set; }
    }
}