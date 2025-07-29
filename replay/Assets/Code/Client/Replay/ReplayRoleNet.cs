namespace KillCam.Client.Replay {
    public class ReplayRoleNet : IClientRoleNet {
        public int Id;
        public CharacterStateData Data;

        public int GetId() {
            return Id;
        }

        public bool IsClientOwned() {
            return false;
        }

        public bool IsControlTarget() {
            return false;
        }

        public CharacterStateData GetData() {
            return Data;
        }

        public void Send<T>(T data) where T : INetworkMsg { }
    }
}