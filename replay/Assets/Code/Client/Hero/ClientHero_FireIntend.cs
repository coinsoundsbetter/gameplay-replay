using Unity.Mathematics;

namespace KillCam.Client {
    
    /// <summary>
    /// 开火意图
    /// </summary>
    public class ClientHero_FireIntend : SystemBase {
        
        protected override void OnCreate() {
            AddBuffer<HeroFireIntentElem>();
        }

        protected override void OnTick() {
            var tick = GetDataRO<NetworkTime>().Tick;
            var identifier = GetDataRO<HeroIdentifier>();
            if (!identifier.IsControlTarget) {
                return;
            }

            var isPressFire = GetDataRO<HeroInputState>().IsFirePressed;
            ref var wpn = ref GetDataRef<HeroWeaponState>();
            if (!isPressFire || wpn.CooldownLeft > 0) {
                return;
            }

            wpn.CooldownLeft = 0.5f;
            wpn.LocalShotSeq++;

            var kinematic = GetDataRO<HeroKinematicState>();
            float3 fwd = math.forward(kinematic.Rotation);

            ref var intentBuffer = ref GetBuffer<HeroFireIntentElem>();
            intentBuffer.Add(new HeroFireIntentElem() {
                Tick = tick,
                LocalShotSeq = wpn.LocalShotSeq,
                Dir = fwd,
                Origin = kinematic.Position,
            });
                
            Send(new C2S_CmdFire() {
                FireTick = tick,
                FireOrigin = kinematic.Position + fwd * 1f,
                FireDir = fwd,
                FireId = wpn.LocalShotSeq,
            });
        }
    }
}