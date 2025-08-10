namespace KillCam.Client {
    public class HeroFire : Feature {
        
        public override bool OnShouldActivate() {
            if (!HasWorldData<UserInputData>()) {
                return false;
            }

            var data = GetWorldDataRO<UserInputData>();
            if (!data.IsFirePressed) {
                return false;
            }

            if (!HasWorldData<CameraData>()) {
                return false;
            }

            var uHero = GetDataManaged<UnityHeroLink>();
            if (uHero == null || uHero.Actor == null) {
                return false;
            }

            return true;
        }

        protected override void OnTickActive() {
            var fireTarget = GetDataManaged<UnityHeroLink>().Actor.GetFireTarget();
            var cameraData = GetWorldDataRO<CameraData>();
            Send(new ProjectileFire() {
                ClientLocalTick = GetTick(),
                Origin = fireTarget.position,
                Direction = (cameraData.aimTarget - fireTarget.position).normalized,
            });
            
            GetWorldFeature<ProjectileManager>().SpawnBullet(new BulletInitData() {
                
            });
        }
    }
}