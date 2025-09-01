using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

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
    
    public class RefStorageBuffer<T> : RefStorageBase where T : unmanaged, IBufferElement {
        public DynamicBuffer<T> Value;
        public ref DynamicBuffer<T> GetRef() => ref Unsafe.AsRef(in Value);

        public override void Dispose() {
            Value.Dispose();
        }
    }

    public struct DynamicBuffer<T> : IDisposable where T : unmanaged, IBufferElement {
        private NativeList<T> buffer;

        public DynamicBuffer(int capacity, Allocator allocator) {
            buffer = new NativeList<T>(capacity, allocator);
        }

        public T this[int index] {
            get => buffer[index];
            set => buffer[index] = value;
        }

        public int Length => buffer.Length;

        public void Add(in T element) {
            buffer.Add(in element);
        }

        public void RemoveAt(int index) {
            buffer.RemoveAt(index);
        }

        public void Clear() {
            buffer.Clear();
        }

        public void Dispose() {
            if (buffer.IsCreated) {
                buffer.Dispose();
            }
        }
    }
}