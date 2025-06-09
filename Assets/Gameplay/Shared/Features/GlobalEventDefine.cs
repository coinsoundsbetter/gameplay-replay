using System;

public static class WorldEventDefine {
    
    public struct RoleNetSpawn : INetSpawnMsg<RoleNet> {
        public bool IsSpawn { get; set; }
        public RoleNet Obj { get; set; }
    }
    
    public struct NetObjSpawn {
        public bool IsSpawn { get; set; }
        public Type ObjType { get; set; }
    }
    
    public struct GameNetSpawn : INetSpawnMsg<GameStateNet> {
        public bool IsSpawn { get; set; }
        public GameStateNet Obj { get; set; }
    }

    public struct RequestLogin {
        public string Name;
        public string Token;
    }
}

public interface INetSpawnMsg<T> {
    bool IsSpawn { get; set; }
    public T Obj { get; set; }
}