public class Launch {
    public static Launch Singleton { get; private set; }
    public static void Create() {
        Singleton = new Launch();
    }

    public static void Clean() {
        Singleton = null;
    }
    
    public bool StartAsServer;
    public bool StartAsClient;
    public bool UseReplay;
    public GameMode StartGameMode;
}