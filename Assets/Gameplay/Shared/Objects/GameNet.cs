using FishNet.Object.Synchronizing;

public class GameNet : NetworkObj {
    public readonly SyncVar<GameState> GameState = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public readonly SyncVar<GameMode> GameMode = new(new SyncTypeSettings(WritePermission.ServerOnly));
}

