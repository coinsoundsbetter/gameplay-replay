
public static class WorldEventDefine {
    public struct NetObjSpawn {
        public bool IsSpawn { get; set; }
        public NetworkObj Obj { get; set; }
    }

    public struct RequestLogin {
        public string Name;
        public string Token;
    }
}