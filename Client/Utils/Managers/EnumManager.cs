using System;

namespace Blaze.Utils.Managers
{
    internal static class EnumManager
    {
        internal static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));
            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        internal static T Next<T>(this T src, int amount) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));
            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(Arr, src) + amount;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        internal static T Previous<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));
            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(Arr, src) - 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        internal static T Previous<T>(this T src, int amount) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));
            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(Arr, src) - amount;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
