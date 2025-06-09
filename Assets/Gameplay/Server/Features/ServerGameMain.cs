public class ServerGameMain : Feature, IUpdateable {
    private WorldEvents worldEvents;
    private ServerFishNet serverFishNet;
    
    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        serverFishNet = link.RequireFeature<ServerFishNet>();
        worldEvents.Register<ServerWorldEvents.GameSpawn>(OnGameSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<ServerWorldEvents.GameSpawn>(OnGameSpawn);
    }

    public void OnUpdate() {
        
        
    }

    private void OnGameSpawn(ServerWorldEvents.GameSpawn ent) {
        
    }

    public void StartGame() {
        serverFishNet.StartConnect();
        serverFishNet.SpawnGame(GameMode.Battle1V1);
    }
}