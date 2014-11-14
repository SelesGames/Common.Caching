using System;

namespace Common.Caching
{
    public interface IExtendedCache<TKey, TResult>
    {
        TResult GetOrAdd(TKey key, Func<TKey, TResult> valueFactory);
    }
}