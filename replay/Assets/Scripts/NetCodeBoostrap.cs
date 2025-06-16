using Unity.NetCode;

public class NetCodeBoostrap : ClientServerBootstrap {
    
    public override bool Initialize(string defaultWorldName) {
        return false;
    }
}