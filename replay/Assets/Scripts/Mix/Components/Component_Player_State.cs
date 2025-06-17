using Unity.Entities;
using UnityEngine;

namespace Mix
{
    public struct PlayerInput : IComponentData {
        public Vector2 Move;
    }
    
    public struct PlayerSpawner : IComponentData { }
}