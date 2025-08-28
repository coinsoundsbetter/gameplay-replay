using FishNet.Managing;

namespace KillCam.Server {
    [UnityEngine.Scripting.Preserve]
    public class ServerEntry : ServerEntryBase {
        private readonly NetworkServer networkServer;

        public ServerEntry(NetworkManager manager) : base(manager) {
            networkServer = new NetworkServer(manager);
        }
        
        protected override void OnBeforeStart() {
            world.AddData(new NetworkTime());
            world.AddData(new NetworkData());
            world.SetNetworkContext(networkServer);
            world.AddFeature(networkServer);
            networkServer.Start(() => {
                world.AddFeature<Server_SpawnHeroSystem>();
                world.AddFeature<NetMessageHandle>();
                world.AddFeature<StateSnapshot>();
            });
        }

        protected override void OnAfterDestroy() {
            networkServer.Stop();
        }
    }
}