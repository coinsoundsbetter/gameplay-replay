public class LaunchData {
    public static LaunchData Instance { get; set; }

    public bool IsClient;
    public bool IsServer;
    public ushort Port = 7888;

    public static void Create() {
        Instance = new LaunchData();
    }
}