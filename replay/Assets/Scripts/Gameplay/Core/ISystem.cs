namespace Gameplay.Core {
    public interface ISystem {
        void OnCreate() { }
        void OnDestroy() { }
        void Update(ref SystemState state) { }
    }
}