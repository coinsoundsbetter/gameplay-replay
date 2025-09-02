using System;

namespace Gameplay {
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public sealed class MessageIdAttribute : Attribute {
        public int Id { get; }
        public MessageIdAttribute(int id) => Id = id;
    }
}