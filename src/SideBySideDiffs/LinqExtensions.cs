using System;
using System.Collections.Generic;
using System.Linq;

namespace SideBySideDiffs
{
    public static class LinqExtensions
    {
        // http://stackoverflow.com/a/7403212/1363815
        public static IEnumerable<TAccumulate> Scan<TSource, TAccumulate>(this IEnumerable<TSource> source,
            TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            foreach (var item in source)
            {
                seed = accumulator(seed, item);
                yield return seed;
            }
        }

        public static IEnumerable<TSource> Scan<TSource>(this IEnumerable<TSource> source,
                Func<TSource, TSource, TSource> accumulator)
        {
            var previous = source.FirstOrDefault();
            foreach (var item in source.Skip(1))
            {
                previous = accumulator(previous, item);
                yield return previous;
            }
        }

    }
}