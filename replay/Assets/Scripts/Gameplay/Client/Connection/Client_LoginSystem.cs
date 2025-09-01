using FishNet.Broadcast;
using Gameplay.Core;
using Unity.Collections;

namespace Gameplay.Client {
    
    public struct LoginRequest : IBroadcast {
        public FixedString32Bytes PlayerName;
    }
    
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