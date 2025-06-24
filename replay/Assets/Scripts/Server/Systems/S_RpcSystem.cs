using Unity.Entities;

namespace KillCam
{
    /// <summary>
    /// 服务器上告知客户端的消息列表
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(PresentationSystemGroup))]
    public partial class S_RpcSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var data = SystemAPI.ManagedAPI.GetSingleton<NetRpc>();
            while (data.RpcList.Count > 0)
            {
                var now = data.RpcList.Dequeue();
                SystemAPI.ManagedAPI.GetSingleton<NetChannels>().Rpc(now.Item1, now.Item2);
            }
        }
    }
}