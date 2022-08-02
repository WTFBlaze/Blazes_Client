using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Utils.VRChat
{
    internal static class USpeakerUtils
    {
        internal static void SetBitrate(BitRate rate)
        {
            PlayerUtils.CurrentUser().field_Private_USpeaker_0.field_Public_BitRate_0 = rate;
        }

        internal static BitRate GetBitRate()
        {
            return PlayerUtils.CurrentUser().field_Private_USpeaker_0.field_Private_BitRate_0;
        }

        internal static void SetGain(float gain)
        {
            USpeaker.field_Internal_Static_Single_1 = gain;
        }

        internal static float GetGain()
        {
            return USpeaker.field_Internal_Static_Single_1;
        }
    }
}
