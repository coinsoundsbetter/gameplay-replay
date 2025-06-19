using Unity.Entities;

namespace KillCam
{
    /// <summary>
    /// 客户端向服务器的上传消息列表
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateAfter(typeof(PresentationSystemGroup))]
    public partial class C_SendSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var data = SystemAPI.ManagedAPI.GetSingleton<SendQueue>();
            while (data.SendList.Count > 0)
            {
                var now = data.SendList.Dequeue();
                SystemAPI.ManagedAPI.GetSingleton<NetChannels>().Handle(now);
            }
        }
    }
}