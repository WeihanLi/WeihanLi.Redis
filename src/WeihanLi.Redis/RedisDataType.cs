namespace WeihanLi.Redis
{
    /// <summary>
    /// Redis 数据类型
    /// </summary>
    public enum RedisDataType : byte
    {
        Cache,
        Counter,
        Firewall,
        RedLock,
        Hash,
        Dictionary,
        List,
        Queue,
        Stack,
        Set,
        SortedSet,
        Rank
    }
}
