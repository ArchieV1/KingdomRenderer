using System;
using System.Collections.Generic;

namespace KingdomRenderer.Shared.ArchieV1.Extensions
{
    public static class EnumExtension
    {
        public static IEnumerable<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }
    }
}