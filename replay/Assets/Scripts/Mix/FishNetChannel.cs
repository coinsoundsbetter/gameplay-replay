using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class FishNetChannel : NetworkBehaviour
{
    public readonly SyncVar<int> PlayerId = new SyncVar<int>();
    public static event Action<FishNetChannel> OnSpawned;
    public static event Action<FishNetChannel> OnDespawn;
    public static event Action<int, byte[]> OnServerReceived;
    public static event Action<byte[]> OnClientReceived; 
    
    public override void OnStartNetwork()
    {
        OnSpawned?.Invoke(this);
    }

    public override void OnStopNetwork()
    {
        OnDespawn?.Invoke(this);
    }

    [ServerRpc]
    public void Send(byte[] clientSend)
    {
        OnServerReceived?.Invoke(PlayerId.Value, clientSend);
    }

    [ObserversRpc]
    public void Rpc(byte[] serverRpc)
    {
        OnClientReceived?.Invoke(serverRpc);
    }
}