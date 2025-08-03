using FishNet.Managing;

namespace KillCam.Server {
    [UnityEngine.Scripting.Preserve]
    public class ServerBoostrap : ServerInitialize {
        private readonly Network network;

        public ServerBoostrap(NetworkManager manager) : base(manager) {
            network = new Network(manager);
        }

        public override void OnCreate() {
            world.Add(network);
            network.Start(() => { AddFeatures(); });
        }

        public override void OnDestroy() {
            network.Stop();
        }

        private void AddFeatures() {
            world.Add(new ActorManager());
            world.Add(new HeroManager());
            world.Add(new Server_C2SHandle());
            world.Add(new StateSnapshot());
        }
    }
}