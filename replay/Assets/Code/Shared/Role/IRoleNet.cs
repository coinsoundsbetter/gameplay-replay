namespace KillCam
{
    public interface IRoleNet
    {
        int GetId();
        bool IsClientOwned();
        bool IsControlTarget();
        RoleStateSnapshot GetData();
    }

    public interface IClientRoleNet : IRoleNet
    {
        void Send(INetworkSerialize data);
    }

    public interface IServerRoleNet : IRoleNet
    {
        void SetServerSyncData(RoleStateSnapshot syncData);
        void Rpc(INetworkSerialize data);
    }
}