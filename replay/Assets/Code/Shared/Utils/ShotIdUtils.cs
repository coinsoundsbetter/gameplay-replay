namespace KillCam {
    public static class ShotIdUtils {
        // 计算一个可预测/可对齐的射击 ID（客户端与服务器一致）
        public static uint Compute(int ownerNetId, uint localSeq, uint tick) {
            unchecked {
                uint h = 2166136261u;
                h = (h ^ (uint)ownerNetId) * 16777619u;
                h = (h ^ (uint)localSeq) * 16777619u;
                h = (h ^ (uint)tick) * 16777619u;
                return h;
            }
        }
    }
}