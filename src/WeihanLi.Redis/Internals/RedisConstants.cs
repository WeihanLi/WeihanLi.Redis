namespace WeihanLi.Redis.Internals
{
    public static class RedisConstants
    {
        #region KeyPrefix

        internal const string CachePrefix = "String:Cache";

        internal const string CounterPrefix = "String:Counter";

        internal const string FirewallPrefix = "String:Firewall";

        internal const string RedLockPrefix = "String:RedLock";

        internal const string HashPrefix = "Hash:Cache";

        internal const string DictionaryPrefix = "Hash:Dictionary";

        internal const string ListPrefix = "List:Cache";

        internal const string SetPrefix = "Set:Cache";

        internal const string SortedSetPrefix = "SortedSet:Cache";

        internal const string RankPrefix = "SortedSet:Rank";

        #endregion KeyPrefix
    }
}
