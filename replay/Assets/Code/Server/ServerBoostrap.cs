using FishNet.Managing;

namespace KillCam.Server {
    [UnityEngine.Scripting.Preserve]
    public class ServerBoostrap : ServerBoostrapBase {
        private readonly NetworkServer networkServer;

        public ServerBoostrap(NetworkManager manager) : base(manager) {
            networkServer = new NetworkServer(manager);
        }
        
        protected override void OnBeforeInitialize() {
            world.AddWorldData(new WorldTime());
            world.AddWorldFunc(networkServer, TickGroup.InitializeLogic);
            networkServer.Start(() => {
                world.AddWorldFunc<HeroManager>(TickGroup.InitializeLogic);
                world.AddWorldFunc<NetMessageHandle>(TickGroup.InitializeLogic);
                world.AddWorldFunc<StateSnapshot>(TickGroup.InitializeLogic);
            });
        }

        protected override void OnAfterCleanup() {
            networkServer.Stop();
        }
    }
}