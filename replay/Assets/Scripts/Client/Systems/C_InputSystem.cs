using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct C_InputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputState>();
            state.RequireForUpdate<LocalConnectState>();
            state.RequireForUpdate<NetChannels>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var localConnState = SystemAPI.GetSingleton<LocalConnectState>();
            if (localConnState.State != NetConnectState.Connected)
            {
                return;
            }
            
            // 刷新输入状态
            var moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            var inputState = new PlayerInputState()
            {
                Move = moveInput,
            };
            SystemAPI.SetSingleton(inputState);
            
            // 发送给服务器
            var net = SystemAPI.ManagedAPI.GetSingleton<NetChannels>();
            net.Send(new C2S_PlayerInputState()
            {
                Data = inputState
            });
        }
    }
}