namespace KillCam.Editor {
    // Runtime assembly
    public static class BattleWorldDebugRegistry
    {
        public static BattleWorld Client {
            get {
                return BattleInitializer.GetWorld(WorldFlag.Client);
            }
        }

        public static BattleWorld Server  {
            get {
                return BattleInitializer.GetWorld(WorldFlag.Server);
            }
        }
    }
}