using FishNet.Managing;

namespace KillCam.Server {
    [UnityEngine.Scripting.Preserve]
    public class ServerEntry : ServerEntryBase {
        private readonly NetworkServer networkServer;

        public ServerEntry(NetworkManager manager) : base(manager) {
            networkServer = new NetworkServer(manager);
        }
        
        protected override void OnBeforeStart() {
            world.SetupData(new WorldTime());
            world.SetNetworkContext(networkServer);
            world.SetupFeature(networkServer);
            networkServer.Start(() => {
                world.SetupFeature<HeroManager>();
                world.SetupFeature<NetMessageHandle>();
                world.SetupFeature<StateSnapshot>();
            });
        }

        protected override void OnAfterDestroy() {
            networkServer.Stop();
        }
    }
}