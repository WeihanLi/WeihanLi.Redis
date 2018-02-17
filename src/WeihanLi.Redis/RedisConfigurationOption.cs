namespace WeihanLi.Redis
{
    public class RedisConfigurationOption
    {
        public int Port { get; set; } = 6379;

        public string Host { get; set; } = "127.0.0.1";

        public string Password { get; set; }

        public int Database { get; set; }

        public bool Ssl { get; set; }
    }
}
