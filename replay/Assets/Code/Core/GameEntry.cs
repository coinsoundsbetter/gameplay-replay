using FishNet.Managing;

namespace KillCam {
    /// <summary>
    /// 游戏必备的入口
    /// </summary>
    public abstract class GameEntry {
        protected BattleWorld world { get; private set; }
        
        public void Start(BattleWorld myWorld) {
            world = myWorld;
            OnBeforeStart();
        }

        public void End() {
            OnAfterDestroy();
        }
        
        protected abstract void OnBeforeStart();
        protected abstract void OnAfterDestroy();
    }
    
    public abstract class ClientEntryBase : GameEntry {
        protected ClientEntryBase(NetworkManager manager) { }
    }
    
    public abstract class ServerEntryBase : GameEntry {
        protected ServerEntryBase(NetworkManager manager) { }
    }
    
    public abstract class ReplayEntryBase : GameEntry { }
}