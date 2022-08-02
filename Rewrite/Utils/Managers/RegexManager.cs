using System.Text.RegularExpressions;

namespace Blaze.Utils.Managers
{
    public static class RegexManager
    {
        public static bool IsValidWorldID(string input)
        {
            return Regex.IsMatch(input, @"(^$|offline|(wrld|wld)_[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})");
        }

        public static bool IsValidAvatarID(string input)
        {
            return Regex.IsMatch(input, @"avtr_[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}");
        }

        public static bool IsValidUserID(string input)
        {
            if (Regex.IsMatch(input, @"[0-9,a-z,A-Z]{10}")) return true; // Old VRChat User ID Format
            else return Regex.IsMatch(input, @"usr_[0-9,a-z,A-Z]{8}-[0-9,a-z,A-Z]{4}-[0-9,a-z,A-Z]{4}-[0-9,a-z,A-Z]{4}-[0-9,a-z,A-Z]{12}"); // Current VRChat User ID Format
        }

        public static bool IsValidHexCode(string input)
        {
            return Regex.IsMatch(input, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
        }
    }
}
