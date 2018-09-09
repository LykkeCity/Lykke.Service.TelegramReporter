using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.TelegramReporter.Services.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> SelectManySafe<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            return source.SelectMany(o => selector(o) ?? Enumerable.Empty<TResult>());
        }
    }
}
