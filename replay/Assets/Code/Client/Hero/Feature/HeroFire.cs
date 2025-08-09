namespace KillCam.Client {
    public class HeroFire : Feature {
        public override bool OnShouldTick() {
            if (!HasWorldData<InputData>()) {
                return false;
            }

            var data = GetWorldDataRO<InputData>();
            return data.IsFirePressed;
        }

        protected override void OnTickActive() {
            Send(new ProjectileFire() {
                ClientLocalTick = GetTick(),
            });
        }
    }
}