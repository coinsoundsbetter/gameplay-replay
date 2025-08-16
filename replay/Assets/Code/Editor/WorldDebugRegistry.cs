namespace KillCam.Editor {
    // Runtime assembly
    public static class BattleWorldDebugRegistry
    {
        public static BattleWorld Client { get; private set; }
        public static BattleWorld Server { get; private set; }

        public static void Register(BattleWorld world)
        {
            if (world.HasFlag(WorldFlag.Client)) {
                Client = world;
            }else if (world.HasFlag(WorldFlag.Server)) {
                Server = world;
            }
        }

        public static void Unregister(BattleWorld world)
        {
            if (Client == world) Client = null;
            if (Server == world) Server = null;
        }
    }

}