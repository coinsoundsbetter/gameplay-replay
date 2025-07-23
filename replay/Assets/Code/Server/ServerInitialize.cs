using FishNet.Managing;

namespace KillCam.Server
{
    public class ServerInitialize : InitializeFeature
    {
        private readonly Server_Network network;
        
        public ServerInitialize(NetworkManager mgr)
        {
            network = new Server_Network(mgr);
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
            world.Add(new Server_CharacterManager());
            world.Add(new Server_C2SHandle());
            world.Add(new Server_StateSnapshot());
        }
    }
}