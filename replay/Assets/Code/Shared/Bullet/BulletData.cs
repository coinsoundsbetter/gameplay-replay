using Unity.Mathematics;

namespace KillCam {

    public struct PredictState {
        public bool IsInPredict;
    }
    
    public struct BulletState {
        public float3 Position;
        public float3 Velocity;
        public float Lifetime;
        public int OwnerId;
        public uint ShotId;
    }
}