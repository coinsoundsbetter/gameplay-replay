using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;

namespace KillCam.Server
{
    public class BaseFeature_ServerLogin : Feature
    {
        private readonly NetworkManager mgr;
        private int playerIdAllocator;

        public BaseFeature_ServerLogin(NetworkManager manager)
        {
            mgr = manager;
        }
        
        public override void OnCreate()
        {
            mgr.ServerManager.RegisterBroadcast<Login>(OnLogin);
        }

        public override void OnDestroy()
        {
            mgr.ServerManager.UnregisterBroadcast<Login>(OnLogin);
        }

        private void OnLogin(NetworkConnection conn, Login info, Channel channel)
        {
            world.Get<BaseFeature_ServerSpawn>().SpawnRole(conn, ++playerIdAllocator);
        }
    }
}