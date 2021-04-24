namespace PidPlugin.Settings
{
    public class PidPluginSettings
    {
        public string   BaseUrl                  { get; set; }
        public int      TimeoutInMinutes         { get; set; }
        public string   SubscriptionKey          { get; set; }
        public int      CacheExpirationInMinutes { get; set; }
    }
}
