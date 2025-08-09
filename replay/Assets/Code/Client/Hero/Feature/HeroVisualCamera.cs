using UnityEngine;

namespace KillCam.Client {
    public class HeroVisualCamera : Feature {
        private CameraDataSource link;
        
        public override bool OnShouldTick() {
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
                pitch = 20f,
                pitchRange = new Vector2(-40f, 55f),
                yaw = 0,
                offsetFromTarget = new Vector3(0, 3, -5),
            });
        }

        protected override void OnTickActive() {
            var inputData = Owner.GetDataReadOnly<HeroInputData>();
            link.pitch += -inputData.Pitch * (float)TickDelta * 300f;
        }
    }
}