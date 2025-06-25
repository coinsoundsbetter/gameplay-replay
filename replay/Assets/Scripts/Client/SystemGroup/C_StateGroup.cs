using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    public partial class C_StateGroup : ComponentSystemGroup
    {
        private FishNet.Managing.NetworkManager manager;
        private SystemHandle netStateSystemHandle;
        public int LocalTick;
        public int RemoteTick;

        protected override void OnCreate()
        {
            base.OnCreate();
            netStateSystemHandle = World.GetOrCreateSystem<C_UpdateNetStateSystem>();
            manager = FishNet.InstanceFinder.NetworkManager;
            FishNetChannel.OnClientReceived += OnClientReceived;
        }

        protected override void OnDestroy()
        {
            FishNetChannel.OnClientReceived -= OnClientReceived;
        }

        // 每隔0.016ms更新一次
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
        
        private void OnClientReceived(byte[] data)
        {
           
        }
    }
}