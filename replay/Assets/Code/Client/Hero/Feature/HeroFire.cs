namespace KillCam.Client {
    public class HeroFire : Feature {
        public override bool OnShouldActivate() {
            if (!HasWorldData<UserInputData>()) {
                return false;
            }

            var data = GetWorldDataRO<UserInputData>();
            return data.IsFirePressed;
        }

        protected override void OnTickActive() {
            Send(new ProjectileFire() {
                ClientLocalTick = GetTick(),
            });
        }
    }
}