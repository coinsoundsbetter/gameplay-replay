using FishNet.Serializing;
using Unity.Entities;

namespace KillCam
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class S_HandleNetworkMsgSystem : SystemBase
    {
        protected override void OnCreate()
        {
            FishNetChannel.OnServerReceived += OnClientRequest;
        }

        protected override void OnUpdate()
        {
            FishNetChannel.OnServerReceived -= OnClientRequest;
        }
        
        private void OnClientRequest(int playerId, byte[] arg2)
        {
            var reader = new Reader();
        }
    }
}