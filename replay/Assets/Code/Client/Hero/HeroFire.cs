namespace KillCam.Client {
    public class HeroFire : Feature {
        private Projectiles projectiles;

        protected override void OnCreate() {
            projectiles = GetWorldFeature<Projectiles>();
        }

        public override bool OnShouldActivate() {
            var inputData = GetDataRO<HeroInputData>();
            if (!inputData.IsFirePressed) {
                return false;
            }

            return true;
        }

        protected override void OnTickActive() {
            ref var fireData = ref GetDataRW<HeroFireData>();
            fireData.FireCd -= TickDeltaFloat;
            if (fireData.FireCd > 0) {
                return;
            }

            fireData.FireId++;
            fireData.FireCd = 0.2f;
            
            var fireTarget = GetDataManaged<UnityHeroLink>().Actor.GetFireTarget();
            var cameraData = GetWorldDataRO<CameraData>();
            
            // 预测生成子弹
            projectiles.SpawnBullet(new BulletInitData() {
                burstSpeed = 300f,
                direction = (cameraData.aimTarget - fireTarget.position).normalized,
                origin = fireTarget.position,
                maxDistance = 100f,
            });
            
            // 开火请求 
            Send(new C2S_CmdFire() {
                FireTick = GetTick(),
                FireOrigin = fireTarget.position,
                FireDir = (cameraData.aimTarget - fireTarget.position).normalized,
                FireId = fireData.FireId,
            });
        }
    }
}