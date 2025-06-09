using UnityEngine;

public class ClientWorldEvents {
    public struct GameSpawn {
        public GameState GameState;
        public GameMode GameMode;
    }
    
    public struct GameDespawn { }

    public struct RoleSpawn {
        public int RoleId;
        public Vector3 Pos;
        public Quaternion Rot;
    }
    
    public struct RoleDespawn {
        public int RoleId;
    }
}