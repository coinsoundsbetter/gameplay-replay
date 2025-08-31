using UnityEngine;

namespace KillCam.Client {
    public class HeroVisualCamera : SystemBase {
        private CameraDataSource link;
        private HeroCameraSO config;

        protected override void OnCreate() {
            config = Resources.Load<HeroCameraSO>("HeroCameraSO");
        }

        public override bool OnShouldActivate() {
            var heroActor = GetDataManaged<UnityHeroLink>();
            if (heroActor.Actor != null) {
                return true;
            }

            return false;
        }

        protected override void OnActivate() {
            var cameraMgr = GetSingletonFeature<CameraManager>();
            cameraMgr.SetDataSource(link = new CameraDataSource() {
                target = GetDataManaged<UnityHeroLink>().Actor.GetCameraTarget(),
                pitch = config.initPitch,
                pitchRange = config.pitchRange,
                yaw = config.initYaw,
                offsetFromTarget = config.offsetFromTarget,
            });
        }

        protected override void OnTick() {
            var inputData = GetDataRO<HeroInputState>();
            link.pitch += -inputData.Pitch * (float)TickDeltaDouble * 300f;
            link.offsetFromTarget = config.offsetFromTarget;
        }
    }
}