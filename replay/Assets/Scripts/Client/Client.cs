using Unity.Entities;
using Unity.NetCode;

namespace KillCam
{
    internal class Client
    {
        private World world;

        internal void Init()
        {
            ClientServerBootstrap.AutoConnectPort = 7979;
            world = ClientServerBootstrap.CreateClientWorld("client");
        }

        internal void Clear()
        {
            world.Dispose();
        }
    }
}