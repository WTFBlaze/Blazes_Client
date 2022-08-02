using System;
using System.Runtime.CompilerServices;

namespace Blaze.Utils.Attributes
{
    [CompilerGenerated]
    [Embedded]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    class NullableContextAttribute : Attribute
    {
        public readonly byte Flag;
        public NullableContextAttribute(byte A_1)
        {
            Flag = A_1;
        }
    }
}
