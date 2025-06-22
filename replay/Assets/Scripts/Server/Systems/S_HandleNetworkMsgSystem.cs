using System.Collections.Generic;
using FishNet.Serializing;
using Unity.Entities;

namespace KillCam
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class S_HandleNetworkMsgSystem : SystemBase
    {
        private readonly Queue<C2SMsg> waitHandles = new ();
        
        protected override void OnCreate()
        {
            FishNetChannel.OnServerReceived += OnClientRequest;
        }

        protected override void OnDestroy()
        {
            FishNetChannel.OnServerReceived -= OnClientRequest;
        }

        protected override void OnUpdate()
        {
            while (waitHandles.Count > 0)
            {
                C2SMsg packet = waitHandles.Dequeue();
                var reader = new Reader();
                var msgType = reader.Read<NetMsg>();
                switch (msgType)
                {
                    
                }
            }
        }
        
        private void OnClientRequest(int playerId, byte[] bytes)
        {
            waitHandles.Enqueue(new C2SMsg()
            {
                PlayerId = playerId,
                Data = bytes,
            });
        }

        #region 协议处理

        #endregion
    }
}