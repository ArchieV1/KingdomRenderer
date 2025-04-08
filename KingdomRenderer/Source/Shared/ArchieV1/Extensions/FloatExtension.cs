using System;

namespace KingdomRenderer.Shared.ArchieV1.Extensions
{
    public static class FloatExtension
    {
        /// <summary>
        /// Is this float a factor of the second
        /// </summary>
        /// <param name="large"></param>
        /// <param name="small"></param>
        /// <param name="precision"></param>
        /// <param name="absolute"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static bool IsFactor(this float large, float small, float precision = 0.001f, bool absolute = false, bool invert = false)
        {
            if (invert)
            {
                return small.IsFactor(large, precision, absolute, false);
            }

            if (absolute)
            {
                return Math.Abs(large) % Math.Abs(small) <= Math.Abs(precision);
            }
            
            return large % small <= precision;
        }
    }
}