using Unity.Entities;
using Unity.NetCode;

namespace KillCam
{
    internal class Server
    {
        private World world;

        internal void Init()
        {
            ClientServerBootstrap.AutoConnectPort = 7979;
            world = ClientServerBootstrap.CreateServerWorld("server");
        }

        internal void Clear()
        {
            world.Dispose();
        }
    }
}