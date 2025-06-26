using Unity.Entities;

namespace KillCam
{
    public partial struct S_RpcSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetTickState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // 每次都会先固定发送当前帧号
            SystemAPI.ManagedAPI.GetSingleton<NetRpc>().Add(new S2C_Tick()
            {
                ServerTick = SystemAPI.GetSingleton<NetTickState>().Local,
            });
            
            var data = SystemAPI.ManagedAPI.GetSingleton<NetRpc>();
            while (data.RpcList.Count > 0)
            {
                var now = data.RpcList.Dequeue();
                SystemAPI.ManagedAPI.GetSingleton<NetChannels>().Rpc(now.Item1, now.Item2);
            }
        }
    }
}