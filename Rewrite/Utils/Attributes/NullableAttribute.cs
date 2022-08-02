using System;
using System.Runtime.CompilerServices;

namespace Blaze.Utils.Attributes
{
    [CompilerGenerated]
    [Embedded]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
    public sealed class NullableAttribute : Attribute
    {
        public readonly byte[] NullableFlags;

        public NullableAttribute(byte[] A_1)
        {
            NullableFlags = A_1;
        }

        public NullableAttribute(byte A_1)
        {
            NullableFlags = new[]
            {
                A_1
            };
        }
    }
}
