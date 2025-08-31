namespace Gameplay.Core {
    public interface ISystem {
        void OnCreate(ref SystemState state) { }
        void OnDestroy() { }
        void Update(ref SystemState state) { }
    }
}