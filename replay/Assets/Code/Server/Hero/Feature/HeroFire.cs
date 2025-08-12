namespace KillCam.Server {
    public class HeroFire : Feature {
        
        protected override void OnTickActive() {
            ref var fireCmdBuffer = ref GetBuffer<C2S_CmdFire>();
            if (fireCmdBuffer.Length == 0) {
                return;
            }
            
            var fireCmd = fireCmdBuffer[0];
            double clientFireTime = fireCmd.FireTick * TickDelta;
            double oneWayDelaySec = GetHalfRTT() / 1000.0;
            double shotTimeSec = clientFireTime - oneWayDelaySec;
            double now = GetNetTime();
        }
    }
}