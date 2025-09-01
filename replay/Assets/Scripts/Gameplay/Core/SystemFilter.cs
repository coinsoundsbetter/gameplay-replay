namespace Gameplay.Core {
    
    [System.Flags]
    public enum SystemFlag {
        None   = 0,
        Client = 1 << 0,
        Server = 1 << 1,
        Replay = 1 << 2,
        All = Client | Server | Replay
    }
    
    public enum SystemFilterMode
    {
        AnyMatch,   // 有交集就算匹配
        Strict      // 必须完全包含
    }
}