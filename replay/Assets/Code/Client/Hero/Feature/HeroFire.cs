namespace KillCam.Client {
    public class HeroFire : Feature {
        public override bool OnShouldTick() {
            if (!World.HasData<InputData>()) {
                return false;
            }

            var data = World.GetDataRO<InputData>();
            return data.IsFirePressed;
        }

        protected override void OnTickActive() {
            World.Send(new ProjectileFire() {
                ClientLocalTick = World.GetTick(),
            });
        }
    }
}