namespace KillCam
{
    public interface IRoleNet
    {
        int GetId();
        bool IsClientOwned();
        bool IsControlTarget();
        CharacterStateData GetData();
    }

    public interface IClientRoleNet : IRoleNet
    {
        void Send(INetworkSerialize data);
    }

    public interface IServerRoleNet : IRoleNet
    {
        void SetServerSyncData(CharacterStateData syncData);
        void Rpc(INetworkSerialize data);
    }
}