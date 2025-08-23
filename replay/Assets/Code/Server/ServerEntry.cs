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
            world.SetupData(new NetworkData());
            world.SetNetworkContext(networkServer);
            world.CreateFeature(networkServer);
            networkServer.Start(() => {
                world.CreateFeature<HeroManager>();
                world.CreateFeature<NetMessageHandle>();
                world.CreateFeature<StateSnapshot>();
            });
        }

        protected override void OnAfterDestroy() {
            networkServer.Stop();
        }
    }
}