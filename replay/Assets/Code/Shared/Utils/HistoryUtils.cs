namespace KillCam {
    public static class HistoryUtils {
        
        public static void PushHistory(ref DynamicBuffer<HeroStateHistory> hist, uint tick, in HeroKinematicState s, int max = 64)
        {
            if (hist.Length >= max) hist.RemoveAt(0);
            hist.Add(new HeroStateHistory { Tick = tick, State = s });
        }

        public static int IndexOfTick(ref DynamicBuffer<HeroStateHistory> hist, int tick)
        {
            for (int i = hist.Length - 1; i >= 0; --i)
                if (hist[i].Tick == tick) return i;
            return -1;
        }
    }
}