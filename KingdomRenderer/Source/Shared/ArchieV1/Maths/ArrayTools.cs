using System.Collections.Generic;
using System.Linq;

namespace KingdomRenderer.Shared.ArchieV1.Maths
{
    public static class ArrayTools
    {
        /// <summary>
        /// Turns a 2d array into a 1d array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> FlattenTo1Dimension<T>(this IEnumerable<T> list) where T : IEnumerable<T>
        {
            List<T> values = new List<T>();

            foreach (T array in list)
            {
                foreach(T item in array)
                {
                    values.Add(item);
                }
            }

            return values;
        }

        /// <summary>
        /// Turns a 1d array into a 2d array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="width">The width of the 2d array.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ExpandTo2Dimension<T>(this IEnumerable<T> input, int width)
        {
            List<IEnumerable<T>> values = new List<IEnumerable<T>>();
            List<T> list = input.ToList();
            int counter = 0;

            while(counter <= list.Count())
            {
                int widthCounter = 0;
                List<T> row = new List<T>();

                while(widthCounter <= width)
                {
                    row.Add(list[counter]);
                    widthCounter++;
                }

                counter += widthCounter;
                values.Add(list);
            }

            return values;
        }
    }
}