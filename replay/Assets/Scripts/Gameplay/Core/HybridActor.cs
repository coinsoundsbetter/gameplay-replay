using UnityEngine;

namespace Gameplay.Core {
    public class HybridActor : MonoBehaviour {
        protected World MyWorld { get; private set; }
        
        public void SetWorld(World world) {
            MyWorld = world;
        }

        public virtual void OnUpdateData() { }
    }
}