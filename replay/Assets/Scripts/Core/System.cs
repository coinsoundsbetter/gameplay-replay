using Arch.Core;

namespace Core
{
    public interface ISystem
    {
        World ECS { get; set; }
        void OnCreate() { }
        void OnUpdate() { }
        void OnDestroy() { }
    }
}