using UnityEngine;

namespace KillCam.Client {
    public class HeroVisualAnim : Feature {
        private UnityHeroLink unityHero;
        
        public override bool OnShouldActivate() {
            unityHero = GetDataManaged<UnityHeroLink>();
            if (unityHero.Actor != null) {
                return true;
            }

            return false;
        }

        protected override void OnTick() {
            var hero = unityHero.Actor;
            var moveData = GetDataRO<HeroMoveData>();
            bool isMoving = moveData.IsMoving;
            hero.SetAnimParam(HeroAnimKeys.IsMove, isMoving);
            hero.SetAnimParam(HeroAnimKeys.MoveDirX, moveData.LocalMoveDirection.x);
            hero.SetAnimParam(HeroAnimKeys.MoveDirZ, moveData.LocalMoveDirection.z);
        }
    }
}