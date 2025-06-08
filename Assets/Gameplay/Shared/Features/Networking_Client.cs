using FishNet.Transporting;

public partial class NetworkRegistry {
    
    private void OnLoginResult(FishBroadcastDefine.LoginResult ent, Channel c) {
        
    }
    
    private void OnLocalClientConnected(ClientConnectionStateArgs args) {
        manager.ClientManager.Broadcast(new FishBroadcastDefine.LoginRequest() {
            Token = "Hello",
            UserName = "Coin",
        });
    }
}