using Unity.Entities;

namespace KillCam
{
    public partial class C_StateGroup : ComponentSystemGroup
    {
        public int LocalTick;
        public int RemoteTick;
        
        // 每隔0.016ms更新一次
        protected override void OnUpdate()
        { 
            var tickState = SystemAPI.GetSingletonRW<NetTickState>();
            base.OnUpdate();
        }
    }
}