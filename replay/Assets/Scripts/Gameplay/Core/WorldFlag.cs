using System;

namespace Gameplay.Core {
    [Flags]
    public enum WorldFlag
    {
        Default = 0,        // 默认（未指定）
        Client  = 1 << 0,   // 客户端
        Server  = 1 << 1,   // 服务端
        Replay  = 1 << 2,   // 回放
        All = Client | Server | Replay
    }
}