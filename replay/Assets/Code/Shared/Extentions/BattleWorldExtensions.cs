using System;

namespace KillCam {
    public static class BattleWorldExtensions {
        public static uint GetTick(this BattleWorld world) {
            return world.GetWorldDataRO<WorldTime>().Tick;
        }
    }
}