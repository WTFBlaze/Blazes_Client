using System.Collections.Generic;
using System.Linq;

namespace Blaze.Utils.Managers
{
    public static class ListManager
    {
        public static void MoveItemAtIndexToFront<T>(this List<T> list, int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            list.Insert(0, item);
        }

        public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
