public class ClientGameState : Feature {
    public GameState CurrentGameState { get; set; }
    public int CommandFrameIndex { get; private set; }
}

public enum GameState {
    Waiting = 0,
    Gaming = 1,
}