namespace WeihanLi.Redis.Internals
{
    internal static class RedisConstants
    {
        #region KeyPrefix

        public const string CachePrefix = "String:Cache";

        public const string CounterPrefix = "String:Counter";

        public const string FirewallPrefix = "String:Firewall";

        public const string RedLockPrefix = "String:RedLock";

        public const string HashPrefix = "Hash:Cache";

        public const string HashCounterPrefix = "Hash:HashCounter";

        public const string DictionaryPrefix = "Hash:Dictionary";

        public const string EventStorePrefix = "Hash:EventStore";

        public const string ListPrefix = "List:Cache";

        public const string QueuePrefix = "List:Queue";

        public const string StackPrefix = "List:Stack";

        public const string SetPrefix = "Set:Cache";

        public const string SortedSetPrefix = "SortedSet:Cache";

        public const string RankPrefix = "SortedSet:Rank";

        #endregion KeyPrefix
    }
}
