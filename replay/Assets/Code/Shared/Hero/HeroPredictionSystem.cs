using Unity.Mathematics;

namespace KillCam {
    public class HeroPredictionSystem : Feature {
        private const float MoveAcc = 20f;
        private const float Gravity = -9.81f;
        private const float BulletSpeed = 60f;
        
        protected override void OnTick() {
            if (!TryGetData(out NetworkData netData)) {
                return;
            }

            foreach (var actor in GetActors(ActorGroup.Player)) {
                var cmdInput = actor.GetDataReadOnly<HeroInputState>();
                
                // 确定性移动
                ref var kinematicState = ref actor.GetDataReadWrite<HeroKinematicState>();
                ref var history = ref actor.GetBuffer<HeroStateHistory>();
                var velocity = kinematicState.Velocity + new float3(cmdInput.Move.x, 0, cmdInput.Move.y) * MoveAcc * TickDelta;
                velocity.y += Gravity * TickDelta;
                var position = kinematicState.Position + velocity * TickDelta;
                kinematicState = new HeroKinematicState() {
                    Position = position,
                    Velocity = velocity,
                    Rotation = kinematicState.Rotation,
                };
                HistoryUtils.PushHistory(ref history, netData.Tick, kinematicState);
                
                // 预测开火
                ref var fireBuffer = ref actor.GetBuffer<HeroFireEventAtTick>();
                if (cmdInput.IsFirePressed) {
                    fireBuffer.Add(new HeroFireEventAtTick() {
                        Tick = netData.Tick,
                    });
                    
                    // 预测子弹初始姿态
                    float3 bulletDir = math.forward(kinematicState.Rotation);
                }
            }
        }
    }
}