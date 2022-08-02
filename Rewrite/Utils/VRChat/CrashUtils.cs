using System.Runtime.CompilerServices;
using UnityEngine;

namespace Blaze.Utils.VRChat
{
    public static class CrashUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvalid(Vector3 vector)
        {
            return float.IsNaN(vector.x) || float.IsInfinity(vector.x) ||
                   float.IsNaN(vector.y) || float.IsInfinity(vector.y) ||
                   float.IsNaN(vector.z) || float.IsInfinity(vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvalid(Quaternion quaternion)
        {
            return float.IsNaN(quaternion.x) || float.IsInfinity(quaternion.x) ||
                   float.IsNaN(quaternion.y) || float.IsInfinity(quaternion.y) ||
                   float.IsNaN(quaternion.z) || float.IsInfinity(quaternion.z) ||
                   float.IsNaN(quaternion.w) || float.IsInfinity(quaternion.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Clamp(short value, short min, short max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Clamp(byte value, byte min, byte max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBeyondLimit(Vector3 vector, float lowerLimit, float higherLimit)
        {
            if (vector.x < lowerLimit || vector.x > higherLimit)
            {
                return true;
            }

            if (vector.y < lowerLimit || vector.y > higherLimit)
            {
                return true;
            }

            if (vector.z < lowerLimit || vector.z > higherLimit)
            {
                return true;
            }

            return false;
        }
    }
}
