using Unity.Collections;

namespace KillCam {
    /// <summary>
    /// 客户端子弹移动逻辑
    /// </summary>
    public class BulletMoveSystem : Feature {
        
        protected override void OnTick() {
            var delta = TickDelta;
            foreach (var actor in GetActors(ActorGroup.Bullet)) {
                ref var state = ref actor.GetDataReadWrite<BulletState>();
                state.Lifetime -= delta;
            }
        }
    }
}