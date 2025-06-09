using UnityEngine;

public class ServerWorldEvents {
    
    public struct RoleSpawn {
        public int RoleId;
        public Vector3 DefaultPos;
        public Quaternion DefaultRot;
    }
    
    public struct RoleDespawn {
        public int RoleId;
    }
}