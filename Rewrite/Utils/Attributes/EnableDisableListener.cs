using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Blaze.Utils.Attributes
{
    [NullableContext(2)]
    [Nullable(0)]
    public class EnableDisableListener : MonoBehaviour
    {
        public EnableDisableListener(IntPtr obj0) : base(obj0) { }

        [method: HideFromIl2Cpp]
        public event Action OnEnabled;

        [method: HideFromIl2Cpp]
        public event Action OnDisabled;

        private void OnEnable()
        {
            var onEnabled = OnEnabled;
            if (onEnabled == null)
            {
                return;
            }
            onEnabled();
        }

        private void OnDisable()
        {
            var onDisabled = OnDisabled;
            if (onDisabled == null)
            {
                return;
            }
            onDisabled();
        }
    }
}
