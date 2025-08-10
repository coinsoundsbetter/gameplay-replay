namespace KillCam.Client {
    public class HeroFire : Feature {
        private Projectiles projectiles;

        protected override void OnSetup() {
            projectiles = GetWorldFeature<Projectiles>();
        }

        protected override void OnTickActive() {
            if (!CanFire()) {
                return;
            }
            
            ref var fireData = ref GetDataRW<HeroFireData>();
            fireData.FireCd -= (float)TickDelta;
            fireData.FireCd = 0.8f;
            var fireTarget = GetDataManaged<UnityHeroLink>().Actor.GetFireTarget();
            var cameraData = GetWorldDataRO<CameraData>();
            Send(new C2S_CmdFire() {
                ClientLocalTick = GetTick(),
                Origin = fireTarget.position,
                Direction = (cameraData.aimTarget - fireTarget.position).normalized,
            });
            
            // 预测生成子弹
            projectiles.SpawnBullet(new BulletInitData() {
                burstSpeed = 10f,
                direction = (cameraData.aimTarget - fireTarget.position).normalized,
                origin = fireTarget.position,
                maxDistance = 500f,
            });
        }

        private bool CanFire() {
            var inputData = GetDataRO<HeroInputData>();
            if (!inputData.IsFirePressed) {
                return false;
            }

            ref var fireData = ref GetDataRW<HeroFireData>();
            if (fireData.IsInCd()) {
                return false;
            }
            
            return true;
        }
    }
}