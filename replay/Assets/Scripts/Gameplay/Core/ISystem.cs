namespace Gameplay.Core {
    public interface ISystem {
        void OnCreate(ref SystemState state) { }
        void OnDestroy(ref SystemState state) { }
        void Update(ref SystemState state) { }
    }
}