public class LaunchData {
    public static LaunchData Instance { get; set; }

    public bool IsClient;
    public bool IsServer;

    public static void Create() {
        Instance = new LaunchData();
    }
}