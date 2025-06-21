using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using FishNet.Serializing;
using Unity.Collections;
using Unity.Entities;

namespace KillCam
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class C_HandleNetworkMsgSystem : SystemBase
    {
        private readonly Queue<S2CMsg> dataQueue = new Queue<S2CMsg>();
        private NetworkManager manager;
        
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
            
            while (dataQueue.Count > 0)
            {
                var reader = new Reader(dataQueue.Dequeue().Data, manager);
                var msgType = reader.Read<NetMsg>();
                switch (msgType)
                {
                    case NetMsg.S2C_SpawnPlayer:
                        var spawnPlayer = new S2C_NetSpawnPlayer();
                        spawnPlayer.Deserialize(reader);
                        OnS2C_SpawnPlayer(spawnPlayer);
                        break;
                }
            }
        }
        
        private void OnClientReceived(byte[] bytes)
        {
            dataQueue.Enqueue(new S2CMsg()
            {
                Data = bytes,
            });
        }

        private void OnS2C_SpawnPlayer(S2C_NetSpawnPlayer message)
        {
            
        }
    }
}