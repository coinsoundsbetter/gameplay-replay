namespace KillCam {
    public interface IRoleNet {
        int GetId();
        bool IsClientOwned();
        bool IsControlTarget();
        CharacterStateData GetData();
    }

    public interface IClientRoleNet : IRoleNet, INetworkClient { }
    public interface IServerRoleNet : IRoleNet, INetworkServer { }
}