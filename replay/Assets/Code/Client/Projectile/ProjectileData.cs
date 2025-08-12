using Unity.Collections;
using UnityEngine;

namespace KillCam.Client {
    public struct BulletInitData {
        public Vector3 origin;
        public Vector3 direction;
        public float burstSpeed;
        public float maxDistance;
        public AnimationCurve distanceDmg;
    }
    
    public struct ProjectileInstance {
        public Vector3 nowPos;
        public Vector3 speed;
        public float gravity;
        public float hasFlyDis;
        public float totalFlyDis;
        public GameObject objHandle;
    }

    public struct ImpactData : IBufferElement {
        public FixedString32Bytes EffSign;
    }
}