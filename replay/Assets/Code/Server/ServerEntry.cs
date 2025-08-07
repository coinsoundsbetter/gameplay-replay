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
            world.SetupFeature(networkServer, TickGroup.InitializeLogic);
            networkServer.Start(() => {
                world.SetupFeature<HeroManager>(TickGroup.InitializeLogic);
                world.SetupFeature<NetMessageHandle>(TickGroup.InitializeLogic);
                world.SetupFeature<StateSnapshot>(TickGroup.InitializeLogic);
            });
        }

        protected override void OnAfterDestroy() {
            networkServer.Stop();
        }
    }
}