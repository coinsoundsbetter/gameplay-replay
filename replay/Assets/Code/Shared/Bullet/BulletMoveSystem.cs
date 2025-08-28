using Unity.Collections;

namespace KillCam {
    /// <summary>
    /// 客户端子弹移动逻辑
    /// </summary>
    public class BulletMoveSystem : Feature {
        
        protected override void OnTick() {
            var delta = TickDelta;
            NativeList<GameplayActor> toDestroy = new NativeList<GameplayActor>(Allocator.Temp);
            foreach (var actor in GetActors(ActorGroup.Bullet)) {
                ref var state = ref actor.GetDataReadWrite<BulletState>();
                state.Lifetime -= delta;
                if (state.Lifetime <= 0) {
                    toDestroy.Add(actor);
                    continue;
                }
                
                state.Position += state.Velocity * delta;
            }

            foreach (var actor in toDestroy) {
                DestroyActor(actor);
            }
            toDestroy.Dispose();
        }
    }
}