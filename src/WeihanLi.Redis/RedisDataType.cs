namespace WeihanLi.Redis
{
    /// <summary>
    /// Redis Data Type
    /// </summary>
    public enum RedisDataType : byte
    {
        Cache = 0,
        Counter = 1,
        Firewall = 2,
        RedLock = 3,
        Hash = 4,
        Dictionary = 5,
        List = 6,
        Queue = 7,
        Stack = 8,
        Set = 9,
        SortedSet = 10,
        Rank = 11,
        HashCounter = 12,
    }
}
