public class ClientGameState : Feature {
    public GameState CurrentGameState { get; set; }
    public int CommandFrameIndex { get; private set; }

    private WorldEvents worldEvents;
    
    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<ClientWorldEvents.GameSpawn>(OnGameSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<ClientWorldEvents.GameSpawn>(OnGameSpawn);
    }

    private void OnGameSpawn(ClientWorldEvents.GameSpawn ent) {
        CurrentGameState = ent.GameState;
    }
}