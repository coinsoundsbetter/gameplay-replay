using FishNet.Object;

namespace Code
{
    public class NetworkParticipant : NetworkBehaviour
    {
        public uint GetLocalTick() => TimeManager.LocalTick;
    }
}