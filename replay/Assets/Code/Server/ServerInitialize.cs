using FishNet.Managing;

namespace KillCam.Server
{
    public class ServerInitialize : InitializeFeature
    {
        private readonly Network network;
        
        public ServerInitialize(NetworkManager mgr)
        {
            network = new Network(mgr);
        }

        public override void OnCreate()
        {
            world.Add(network);
            network.Start(() =>
            {
                AddFeatures();
            });
        }

        public override void OnDestroy()
        {
            network.Stop();
        }
        
        private void AddFeatures()
        {
            world.Add(new ActorManager());
            world.Add(new CharacterManager());
            world.Add(new Server_C2SHandle());
            world.Add(new StateSnapshot());
        }
    }
}