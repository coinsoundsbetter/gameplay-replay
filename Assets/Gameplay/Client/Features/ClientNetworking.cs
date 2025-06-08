using FishNet;
using FishNet.Broadcast;
using FishNet.Managing;
using FishNet.Transporting;

public class ClientNetworking : Feature, IUpdateable {
    private NetworkManager manager;
    public uint LocalTick { get; private set; }
    public uint GameNetTick { get; private set; }

    public int ClientID => manager.ClientManager.Connection.ClientId;
    
    public override void OnInitialize(ref WorldLink link) {
        manager = InstanceFinder.NetworkManager;
        manager.ClientManager.OnClientConnectionState += OnLocalConnectionState;
    }

    public override void OnClear() {
        manager.ClientManager.OnClientConnectionState -= OnLocalConnectionState;
    }
    
    public void OnUpdate() {
        LocalTick = manager.TimeManager.LocalTick;
        GameNetTick = manager.TimeManager.Tick;
    }

    private void OnLocalConnectionState(ClientConnectionStateArgs args) {
        if (args.ConnectionState == LocalConnectionState.Started) {
            manager.ClientManager.Broadcast(new FishBroadcastDefine.LoginRequest() {
                Token = "HeiHei",
                UserName = "Coin",
            });
        }
    }

    public void StartConnect() {
        manager.ClientManager.StartConnection();
    }

    public void BroadcastToServer<T>(T broadcast) where T : struct, IBroadcast {
        manager.ClientManager.Broadcast<T>(broadcast, Channel.Reliable);
    }
}