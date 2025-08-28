namespace KillCam.Client {
    /// <summary>
    /// 弹孔特效的表现
    /// </summary>
    public class ProjectileImpacts : Feature {
        
        protected override void OnTick() {
            ref var impactBuffer = ref GetWorldBuffer<ImpactData>();
            if (impactBuffer.Length == 0) {
                return;
            }

            for (int i = 0; i < impactBuffer.Length; i++) {
                var element = impactBuffer[i];
                // TODO:播放特效
            }
            impactBuffer.Clear();
        }
    }
}