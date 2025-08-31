using System;
using System.Runtime.CompilerServices;

namespace Gameplay.Core {
    public class RefStorageBase : IDisposable {
        public virtual void Dispose() {
        }

        public virtual object GetBoxed() {
            return null;
        }
    }
    
    public class RefStorage<T> : RefStorageBase where T : unmanaged {
        public T Value;
        public ref T GetRef() => ref Unsafe.AsRef(in Value);

        public override object GetBoxed() {
            return Value;
        }

        public override void Dispose() {
            if (Value is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}