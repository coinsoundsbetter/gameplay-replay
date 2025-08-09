namespace KillCam.Client {
    public class HeroVisualMove : Feature {
        private IUnityHero unityHero;

        public override bool OnShouldTick() {
            var link = Owner.GetDataManaged<UnityHeroLink>();
            unityHero = link.Actor;
            if (unityHero != null) {
                return true;
            }

            return false;
        }

        protected override void OnTickActive() {
            var moveData = Owner.GetDataReadOnly<HeroMoveData>();
            unityHero.SetPosition(moveData.Pos);
            unityHero.SetRotation(moveData.Rot);
        }
    }
}