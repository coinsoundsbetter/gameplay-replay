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

            cmd = new EntityCommandBuffer(Allocator.Temp);
            while (dataQueue.Count > 0)
            {
                var reader = new Reader(dataQueue.Dequeue().Data, manager);
                var msgType = (NetMsg)reader.ReadUInt32();
                switch (msgType)
                {
                    case NetMsg.S2C_SpawnPlayer:
                        var spawnPlayer = new S2C_NetSpawnPlayer();
                        spawnPlayer.Deserialize(reader);
                        var ent = cmd.CreateEntity();
                        cmd.AddComponent(ent, spawnPlayer);
                        break;
                }
            }

            cmd.Playback(EntityManager);
            cmd.Dispose();
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