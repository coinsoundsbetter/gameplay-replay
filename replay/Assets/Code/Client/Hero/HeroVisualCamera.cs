using UnityEngine;

namespace KillCam.Client {
    public class HeroVisualCamera : Feature {
        private CameraDataSource link;
        private HeroCameraSO config;

        protected override void OnCreate() {
            config = Resources.Load<HeroCameraSO>("HeroCameraSO");
        }

        public override bool OnShouldActivate() {
            var heroActor = Owner.GetDataManaged<UnityHeroLink>();
            if (heroActor.Actor != null) {
                return true;
            }

            return false;
        }

        protected override void OnActivate() {
            var cameraMgr = GetWorldFeature<CameraManager>();
            cameraMgr.SetDataSource(link = new CameraDataSource() {
                target = Owner.GetDataManaged<UnityHeroLink>().Actor.GetCameraTarget(),
                pitch = config.initPitch,
                pitchRange = config.pitchRange,
                yaw = config.initYaw,
                offsetFromTarget = config.offsetFromTarget,
            });
        }

        protected override void OnTickActive() {
            var inputData = Owner.GetDataReadOnly<HeroInputData>();
            link.pitch += -inputData.Pitch * (float)TickDelta * 300f;
            link.offsetFromTarget = config.offsetFromTarget;
        }
    }
}