using System;

namespace Gameplay.Core {
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DisableCollectorAttribute : Attribute {
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateInGroupAttribute : Attribute
    {
        public Type GroupType { get; }
        public UpdateInGroupAttribute(Type groupType) => GroupType = groupType;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateBeforeAttribute : Attribute
    {
        public Type TargetType { get; }
        public UpdateBeforeAttribute(Type targetType) => TargetType = targetType;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateAfterAttribute : Attribute
    {
        public Type TargetType { get; }
        public UpdateAfterAttribute(Type targetType) => TargetType = targetType;
    }

    /// <summary>执行顺序（最前/默认/最后）</summary>
    public enum SystemOrder { Default = 0, First = -1000, Last = 1000 }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OrderAttribute : Attribute
    {
        public SystemOrder Order { get; }
        public OrderAttribute(SystemOrder order) => Order = order;
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SystemTagAttribute : Attribute
    {
        public SystemFlag Flag { get; }
        public SystemFilterMode Mode { get; }

        public SystemTagAttribute(SystemFlag flag, SystemFilterMode mode = SystemFilterMode.AnyMatch)
        {
            Flag = flag;
            Mode = mode;
        }
    }
}