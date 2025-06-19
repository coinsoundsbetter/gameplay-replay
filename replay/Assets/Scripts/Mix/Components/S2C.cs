using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    public struct S2C_RpcAll : INetCommand {
        
    }
    
    public struct S2C_NetSpawnPlayer : INetCommand {
        public int PlayerId;
        public FixedString32Bytes PlayerName;
        public Vector3 Pos;
        public Quaternion Rot;
    }
    
    public interface INetCommand : IComponentData { }
}