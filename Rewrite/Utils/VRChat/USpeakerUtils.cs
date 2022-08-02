namespace Blaze.Utils.VRChat
{
    public static class USpeakerUtils
    {
        public static void SetBitrate(BitRate rate)
        {
            PlayerUtils.CurrentUser().field_Private_USpeaker_0.field_Public_BitRate_0 = rate;
        }

        public static BitRate GetBitRate()
        {
            return PlayerUtils.CurrentUser().field_Private_USpeaker_0.field_Private_BitRate_0;
        }

        public static void SetGain(float gain)
        {
            USpeaker.field_Internal_Static_Single_1 = gain;
        }

        public static float GetGain()
        {
            return USpeaker.field_Internal_Static_Single_1;
        }
    }
}
