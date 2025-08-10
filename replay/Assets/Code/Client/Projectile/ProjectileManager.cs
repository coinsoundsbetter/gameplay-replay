using UnityEngine;

namespace KillCam.Client {
    public class ProjectileManager : Feature {
        
        public void SpawnBullet(BulletInitData data) {
            
        }
    }

    public struct BulletInitData {
        public Vector3 origin;
        public Vector3 direction;
        public Vector3 burstSpeed;
        public AnimationCurve distanceDmg;
    }
}