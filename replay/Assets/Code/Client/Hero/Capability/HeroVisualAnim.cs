using UnityEngine;

namespace KillCam.Client {
    public class HeroVisualAnim : Capability {
        private UnityHeroLink unityHero;
        
        public override bool OnShouldActivate() {
            unityHero = Owner.GetDataManaged<UnityHeroLink>();
            if (unityHero.Actor != null) {
                return true;
            }

            return false;
        }

        protected override void OnTickActive() {
            var hero = unityHero.Actor;
            var moveData = Owner.GetDataReadOnly<HeroMoveData>();
            bool isMoving = moveData.IsMoving;
            hero.SetAnimParam(HeroAnimKeys.IsMove, isMoving);
            hero.SetAnimParam(HeroAnimKeys.MoveDirX, moveData.LocalMoveDirection.x);
            hero.SetAnimParam(HeroAnimKeys.MoveDirZ, moveData.LocalMoveDirection.z);
        }
    }
}