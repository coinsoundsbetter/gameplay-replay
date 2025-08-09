namespace KillCam {
    public static class Extensions {

        public static bool HasFlag(this WorldFlag worldFlag, WorldFlag check) {
            return (worldFlag & check) != check;
        }
    }
}