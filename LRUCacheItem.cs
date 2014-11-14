
namespace Common.Caching
{
    public class LRUCacheItem<TKey, TValue>
    {
        internal LRUCacheItem(TKey k, TValue v)
        {
            Key = k;
            Value = v;
        }

        public TKey Key { get; private set; }
        public TValue Value { get; internal set; }
    }
}