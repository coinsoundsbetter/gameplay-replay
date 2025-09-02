using Gameplay.Core;

namespace Gameplay.Client {
    
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    [SystemTag(SystemFlag.Client, SystemFilterMode.Strict)]
    public class Client_LoginSystem : SystemBase {
        private bool isSendLogin;
        
        protected override void OnUpdate() {
            if (isSendLogin) {
                return;
            }

            var state = Actors.GetSingleton<ConnectionInfo>();
            if (state.State != ConnectState.Connected) {
                return;
            }
            
            Actors.GetSingletonManaged<NetworkClient>().usePlugin.ClientManager.Broadcast(new LoginRequest() {
                PlayerName = "Coin",
            });
            isSendLogin = true;
        }
    }
}