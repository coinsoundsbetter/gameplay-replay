namespace KillCam.Client.Replay
{
    public class ReplayRoleNet : IClientRoleNet
    {
        public int Id;
        public RoleStateSnapshot Data;
        
        public int GetId()
        {
            return Id;
        }

        public bool IsClientOwned()
        {
            return false;
        }

        public bool IsControlTarget()
        {
            return false;
        }

        public RoleStateSnapshot GetData()
        {
            return Data;
        }

        public void Send(INetworkSerialize data) { }
    }
}