using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class FishNetChannel : NetworkBehaviour
{
    public readonly SyncVar<int> PlayerId = new SyncVar<int>();
    public static event Action<FishNetChannel> OnSpawnAsServer;
    public static event Action<FishNetChannel> OnDespawnAsServer;
    public static event Action<FishNetChannel> OnSpawnAsClient;
    public static event Action<FishNetChannel> OnDespawnAsClient;
    public static event Action<int, byte[]> OnServerReceived;
    public static event Action<byte[]> OnClientReceived; 
    
    public override void OnStartClient()
    {
        OnSpawnAsClient?.Invoke(this);
    }

    public override void OnStopClient()
    {
        OnDespawnAsClient?.Invoke(this);
    }

    public override void OnStartServer()
    {
        OnSpawnAsServer?.Invoke(this);
    }

    public override void OnStopServer()
    {
        OnDespawnAsServer?.Invoke(this);
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