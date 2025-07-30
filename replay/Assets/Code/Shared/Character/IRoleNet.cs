namespace KillCam {
    public interface ICharacterNet {
        int GetId();
        CharacterStateData GetData();
    }

    public interface IClientCharacterNet : ICharacterNet {
        bool IsClientOwned();
        void Send<T>(T message) where T : INetworkMsg;
    }

    public interface IServerCharacterNet : ICharacterNet {
        void Rpc<T>(T message) where T : INetworkMsg;
    }
}