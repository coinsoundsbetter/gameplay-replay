using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using FishNet.Serializing;
using Unity.Entities;

namespace KillCam
{
    public partial class S_HandleNetworkMsgSystem : SystemBase
    {
        private readonly Queue<C2SMsg> waitHandles = new ();
        private NetworkManager manager;
        
        protected override void OnCreate()
        {
            manager = InstanceFinder.NetworkManager;
            FishNetChannel.OnServerReceived += OnClientRequest;
        }

        protected override void OnDestroy()
        {
            FishNetChannel.OnServerReceived -= OnClientRequest;
        }

        protected override void OnUpdate()
        {
            if (waitHandles.Count == 0)
            {
                return;
            }
            
            PoolNetworkEvents();
        }
        
        private void OnClientRequest(int playerId, byte[] bytes)
        {
            waitHandles.Enqueue(new C2SMsg()
            {
                PlayerId = playerId,
                Data = bytes,
            });
        }
    }
}