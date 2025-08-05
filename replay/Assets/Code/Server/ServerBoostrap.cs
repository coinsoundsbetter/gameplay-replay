using FishNet.Managing;

namespace KillCam.Server {
    [UnityEngine.Scripting.Preserve]
    public class ServerBoostrap : ServerBoostrapBase {
        private readonly NetworkServer networkServer;

        public ServerBoostrap(NetworkManager manager) : base(manager) {
            networkServer = new NetworkServer(manager);
        }
        
        protected override void OnBeforeInitialize() {
            var worldActor = MyWorldActor;
            MyWorldActor.SetupData(new WorldTime());
            worldActor.SetupCapability(networkServer, TickGroup.InitializeLogic);
            networkServer.Start(() => {
                worldActor.SetupCapability<HeroManager>(TickGroup.InitializeLogic);
                worldActor.SetupCapability<Server_C2SHandle>(TickGroup.InitializeLogic);
                worldActor.SetupCapability<StateSnapshot>(TickGroup.InitializeLogic);
            });
        }

        protected override void OnAfterCleanup() {
            networkServer.Stop();
        }
    }
}