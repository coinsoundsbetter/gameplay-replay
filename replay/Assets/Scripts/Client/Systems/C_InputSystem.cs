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






/*using Mix;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ClientInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton<PlayerInput>();
            state.RequireForUpdate<NetworkId>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entity = SystemAPI.GetSingletonEntity<NetworkId>();
            if (!SystemAPI.HasComponent<NetworkStreamInGame>(entity))
            {
                return;
            }

            // 得到当前帧的输入
            var frameInput = new PlayerInput()
            {
                Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            };
            SystemAPI.SetSingleton(frameInput);

            // 上传当前帧输入
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var sendEnt = ecb.CreateEntity();
            ecb.AddComponent(sendEnt, new CmdPlayerInput()
            {
                PlayerId = 1,
                Input = frameInput,
            });
            ecb.AddComponent(sendEnt, new SendRpcCommandRequest()
            {
                TargetConnection = Entity.Null,
            });
            ecb.Playback(state.EntityManager);
        }
    }
}*/