public class ServerGameState : Feature, IUpdateable {
    private NetworkRegistry _networkRegistry;
    private uint offsetBeforeStartGame;
    private bool inInGaming;
    public uint CommandIndex { get; private set; }

    public override void OnInitialize(ref WorldLink link) {
        
    }

    public void OnUpdate() {
        if (!inInGaming) {
            return;
        }
        
    }

    public void StartGame() {
        inInGaming = true;
        //offsetBeforeStartGame = _networkRegistry.TickNumber;
        CommandIndex = 0;
    }
}