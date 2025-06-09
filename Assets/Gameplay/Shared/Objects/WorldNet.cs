using FishNet.Object.Synchronizing;

public class GameStateNet : NetworkObj {
    public readonly SyncVar<GameState> gameState;
}
