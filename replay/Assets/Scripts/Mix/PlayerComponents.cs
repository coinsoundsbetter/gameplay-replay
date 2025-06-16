
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Mix {

    public struct LoginRequest : IRpcCommand {
        public FixedString64Bytes PlayerName;
    }
    
    public struct PlayerInput : IComponentData {
        public Vector2 Move;
    }

    public struct CmdPlayerInput : IRpcCommand {
        public uint PlayerId;
        public PlayerInput Input;
    }
    
    

    public readonly partial struct PlayerAspect : IAspect
    {
        public readonly RefRW<LocalTransform> transform;
    }
}