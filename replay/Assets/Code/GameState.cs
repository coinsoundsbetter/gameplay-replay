using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;

public class GameState : NetworkBehaviour, ISerializeAs<GameStateSnapshot>
{
    public readonly SyncVar<GameProcessEnum> GameProcess = new SyncVar<GameProcessEnum>();
    
    public byte[] Serialize()
    {
        var writer = new Writer();
        writer.Write((uint)GameProcess.Value);
        return writer.GetBuffer();
    }

    public GameStateSnapshot Deserialize(byte[] data)
    {
        var result = new GameStateSnapshot();
        var reader = new Reader(data, null);
        result.GameProcess = (GameProcessEnum)reader.ReadUInt32();
        return result;
    }

    public GameStateSnapshot Parse()
    {
        throw new System.NotImplementedException();
    }
}

public enum GameProcessEnum : uint
{
    Undefined = 0,
    WaitingStart = 1,
    Battle = 2,
}

public struct GameStateSnapshot
{
    public GameProcessEnum GameProcess;
}