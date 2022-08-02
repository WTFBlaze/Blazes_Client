using UnityEngine;

namespace Blaze.Utils.Managers
{
    public static class ColorManager
    {
        public static Color HexToColor(string hexCode)
        {
            ColorUtility.DoTryParseHtmlColor(hexCode, out var color);
            return color;
        }

        public static Color32 HexToColor32(string hexCode)
        {
            ColorUtility.DoTryParseHtmlColor(hexCode, out var color);
            return color;
        }

        public static string ColorToHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        public static string Color32ToHex(Color32 color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        public static Color ShiftHueBy(Color color, float amount)
        {
            Color.RGBToHSV(color, out float hue, out float sat, out float val);

            hue += amount;
            sat = 1f;
            val = 1f;

            // convert back to RGB and return the color
            return Color.HSVToRGB(hue, sat, val);
        }

        public static string TextGradient(string input)
        {
            string result = string.Empty;
            Color currentColor = Color.white;
            foreach (char c in input)
            {
                if (c == ' ')
                {
                    result += ' ';
                }
                else
                {
                    currentColor = ShiftHueBy(currentColor, 0.1f);
                    var hexCode = ColorToHex(currentColor);
                    result += $"<color=#{hexCode}>{c}</color>";
                }
            }
            return result;
        }

        public static string TextGradient(string input, ref Color inputColor)
        {
            string result = string.Empty;
            foreach (char c in input)
            {
                if (c == ' ')
                {
                    result += ' ';
                }
                else
                {
                    inputColor = ShiftHueBy(inputColor, 0.1f);
                    var hexCode = ColorToHex(inputColor);
                    result += $"<color=#{hexCode}>{c}</color>";
                }
            }
            return result;
        }
    }
}
