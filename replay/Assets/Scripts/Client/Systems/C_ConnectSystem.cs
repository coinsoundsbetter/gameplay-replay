using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class C_ConnectSystem : SystemBase {
        private NetworkManager manager;

        protected override void OnStartRunning()
        {
            manager = InstanceFinder.NetworkManager;
            manager.ClientManager.OnClientConnectionState += OnConnectState;
            manager.ClientManager.StartConnection();
        }

        protected override void OnStopRunning()
        {
            manager.ClientManager.StopConnection();
            manager.ClientManager.OnClientConnectionState -= OnConnectState;
        }

        protected override void OnUpdate() { }
        
        private void OnConnectState(ClientConnectionStateArgs args) {
            NetConnectState next = NetConnectState.Undefined;
            switch (args.ConnectionState)
            {
                case LocalConnectionState.Started:
                    next = NetConnectState.Connected;
                    break;
                case LocalConnectionState.Stopped:
                    next = NetConnectState.Disconnected;
                    break;
            }
            
            Debug.Log("连接状态: " + args.ConnectionState);

            var cmd = new EntityCommandBuffer(Allocator.Temp);
            var loginReq = cmd.CreateEntity();
            cmd.AddComponent(loginReq, new Event_ConnectState()
            {
                Last = SystemAPI.GetSingletonRW<LocalConnectState>().ValueRW.State,
                Now = next,
            });
            cmd.Playback(EntityManager);
            cmd.Dispose();
            
            SystemAPI.GetSingletonRW<LocalConnectState>().ValueRW.State = next;
        }
    }
}