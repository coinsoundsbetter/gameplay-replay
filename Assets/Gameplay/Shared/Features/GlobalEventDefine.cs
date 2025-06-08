public static class WorldEventDefine {
    public struct RoleNetSpawn {
        public bool IsSpawn;
        public RoleNet RoleNet;
    }

    public struct RequestLogin {
        public string Name;
        public string Token;
    }
}