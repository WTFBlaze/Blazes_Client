using System.Collections.Generic;
using UnityEngine;

namespace Blaze.Utils.Managers
{
    internal static class ColorManager
    {
        internal static Color HexToColor(string hexCode)
        {
            ColorUtility.DoTryParseHtmlColor(hexCode, out var color);
            return color;
        }

        internal static Color32 HexToColor32(string hexCode)
        {
            ColorUtility.DoTryParseHtmlColor(hexCode, out var color);
            return color;
        }

        internal static string ColorToHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        internal static string Color32ToHex(Color32 color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        internal static Color ShiftHueBy(Color color, float amount)
        {
            Color.RGBToHSV(color, out float hue, out float sat, out float val);

            hue += amount;
            sat = 1f;
            val = 1f;

            // convert back to RGB and return the color
            return Color.HSVToRGB(hue, sat, val);
        }

        internal static string TextGradient(string input)
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

        internal static string TextGradient(string input, ref Color inputColor)
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
