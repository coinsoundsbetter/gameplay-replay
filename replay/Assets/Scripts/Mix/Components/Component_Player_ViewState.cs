using Unity.Entities;
using UnityEngine;

namespace Mix {
    public struct PlayerViewTag : IComponentData { }
    
    public interface IPlayerView
    {
        public Animator Anim { get;}
        public CharacterController Cc { get; }
        public Transform Trans { get; }
    }
}