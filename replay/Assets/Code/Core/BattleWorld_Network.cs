namespace KillCam {
    public partial class BattleWorld {
        public INetworkContext NetworkContext;
        
        public void SetNetworkContext(INetworkContext context) {
            NetworkContext = context;
        }
    }
}