namespace P5.WebApi2.Hub
{
    public static class WebApi2Hub
    {
        public static WebApi2HubOptions WebApi2HubOptions { get; set; }
        public static void Use(WebApi2HubOptions options)
        {
            WebApi2HubOptions = options;
        }
    }
}