using Gameplay.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Gameplay.Client {
    /// <summary>
    /// 客户端收集设备输入
    /// </summary>
    [SystemTag(SystemFlag.Client)]
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public class Client_GatherInputSystem : ISystem {
        
        public void OnCreate(ref SystemState state) {
            state.ActorManager.CreateSingleton(new UserInput());
        }

        public void Update(ref SystemState state) {
            ref var input = ref state.ActorManager.GetSingleton<UserInput>();
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            h = h switch {
                > 0 => 1,
                < 0 => -1,
                _ => 0
            };
            v = v switch {
                > 0 => 1,
                < 0 => -1,
                _ => 0
            };
            input.move = new int2((int)h, (int)v);
            input.yaw = Input.GetAxis("Mouse X");
            input.pitch = Input.GetAxis("Mouse Y");
            input.isFirePressed = Input.GetButton("Fire1");
        }
    }

    public struct UserInput : IActorData {
        public int2 move;
        public float pitch;
        public float yaw;
        public bool isFirePressed;
    }
}