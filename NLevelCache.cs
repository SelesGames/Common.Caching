using System;
using System.Collections.Generic;

namespace Common.Caching
{
    public class NLevelCache<TKey, TResult> : IExtendedCache<TKey, TResult>
    {
        LinkedList<IExtendedCache<TKey, TResult>> caches;

        public NLevelCache(IEnumerable<IExtendedCache<TKey, TResult>> caches)
        {
            this.caches = new LinkedList<IExtendedCache<TKey, TResult>>(caches);
        }

        public TResult GetOrAdd(TKey key, Func<TKey, TResult> valueFactory)
        {
            return Get(key, valueFactory, caches.First);
        }

        TResult Get(TKey key, Func<TKey, TResult> valueFactory, LinkedListNode<IExtendedCache<TKey, TResult>> node)
        {
            if (node == null)
                return valueFactory(key);
            else
            {
                var result = node.Value.GetOrAdd(key, _ => Get(key, valueFactory, node.Next));
                return result;
            }
        }
    }
}
