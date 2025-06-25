using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using Unity.Entities;

namespace KillCam
{
    public partial class C_HandleNetworkMsgSystem : SystemBase
    {
        private readonly Queue<S2CMsg> dataQueue = new Queue<S2CMsg>();
        private NetworkManager manager;
        private EntityCommandBuffer cmd;

        protected override void OnCreate()
        {
            manager = InstanceFinder.NetworkManager;
            FishNetChannel.OnClientReceived += OnClientReceived;
        }

        protected override void OnDestroy()
        {
            FishNetChannel.OnClientReceived -= OnClientReceived;
        }

        protected override void OnUpdate()
        {
            if (manager == null || !manager.Initialized)
            {
                return;
            }

            if (dataQueue.Count == 0)
            {
                return;
            }

            PoolNetworkEvents();
        }

        private void OnClientReceived(byte[] bytes)
        {
            dataQueue.Enqueue(new S2CMsg()
            {
                Data = bytes,
            });
        }
    }
}