using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.Mathematics;

namespace Gameplay {
    public class HeroSync : NetworkBehaviour {
        public readonly SyncVar<int> Id = new SyncVar<int>();
        public readonly SyncVar<float> Health = new SyncVar<float>(100f);
        public readonly SyncVar<float3> Position = new SyncVar<float3>();
        public readonly SyncVar<float3> Rotation = new SyncVar<float3>();
    }
}