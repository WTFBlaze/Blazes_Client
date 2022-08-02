﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Utils.Managers
{
    public static class StringManager
    {
        public static string TruncateLongString(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return str;

            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static bool LongerThanCount(this string str, int maxLength)
        {
            int characterCount = 0;
            foreach (char c in str) { characterCount++; }
            if (characterCount >= maxLength) return true;
            return false;
        }
    }
}
