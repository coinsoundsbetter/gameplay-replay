namespace KillCam.Client {
    public class HeroVisualMove : Feature {
        private IUnityHero unityHero;

        public override bool OnShouldActivate() {
            var link = GetDataManaged<UnityHeroLink>();
            unityHero = link.Actor;
            if (unityHero != null) {
                return true;
            }

            return false;
        }

        protected override void OnTick() {
            var moveData = GetDataRO<HeroMoveData>();
            unityHero.SetPosition(moveData.Pos);
            unityHero.SetRotation(moveData.Rot);
        }
    }
}