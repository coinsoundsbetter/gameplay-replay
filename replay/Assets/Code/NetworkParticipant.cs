using FishNet.Object;

public class NetworkParticipant : NetworkBehaviour
{
    public uint GetLocalTick() => TimeManager.LocalTick;
}