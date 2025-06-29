using FishNet.Broadcast;

namespace KillCam
{
    public struct Login : IBroadcast
    {
        public string UserName;
    }
}