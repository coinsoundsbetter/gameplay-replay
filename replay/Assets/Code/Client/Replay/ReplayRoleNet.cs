namespace KillCam.Client.Replay {
    public class ReplayHeroNet : IClientHeroNet {
        public int Id;
        public HeroMoveData Data;

        public int GetId() {
            return Id;
        }

        public bool IsClientOwned() {
            return false;
        }

        public bool IsControlTarget() {
            return false;
        }

        public HeroMoveData GetData() {
            return Data;
        }

        public void Send<T>(T data) where T : INetworkMsg { }
    }
}