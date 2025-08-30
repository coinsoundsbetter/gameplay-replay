using Unity.Mathematics;

namespace KillCam {

    public struct HitBoxCapsule : IActorData {
        public float3 LocalP0;
        public float3 LocalP1;
        public float3 WorldP0;
        public float3 WorldP1;
        public float Radius;
    }
}