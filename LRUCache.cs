using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common.Caching
{
    /// <summary>
    /// Implementation of a Least Recently Used Cache
    /// </summary>
    public class LRUCache<TKey, TValue> : IEnumerable<LRUCacheItem<TKey, TValue>>, IEnumerable
    {
        readonly int capacity;
        readonly object sync = new object();

        readonly LinkedList<LRUCacheItem<TKey, TValue>> list;
        readonly Dictionary<TKey, LinkedListNode<LRUCacheItem<TKey, TValue>>> cache;

        public LRUCache(int capacity)
        {
            if (capacity < 0) throw new ArgumentException("capacity");

            this.capacity = capacity;
            list = new LinkedList<LRUCacheItem<TKey, TValue>>();
            cache = new Dictionary<TKey, LinkedListNode<LRUCacheItem<TKey, TValue>>>(capacity + 1);
        }

        /// <summary>
        /// Gets the value associated with the specified key.  If no key is found, returns Option.None.
        /// </summary>
        /// <returns>An Option type that optionally contains the value associated with the key</returns>
        public Option<TValue> Get(TKey key)
        {
            lock (sync)
            {
                LinkedListNode<LRUCacheItem<TKey, TValue>> node;

                if (!cache.TryGetValue(key, out node))
                    return Option.None<TValue>();

                list.Remove(node);
                list.AddLast(node);

                return Option.Some(node.Value.Value);
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the LRUCache<TKey,TValue>.  
        /// If the number of elements is greater than capacity, returns the evicted item.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The evicted LRU item, or null if nothing was evicted</returns>
        public LRUCacheItem<TKey, TValue> AddOrUpdate(TKey key, TValue value)
        {
            lock (sync)
            {
                LinkedListNode<LRUCacheItem<TKey, TValue>> node;

                // if the cache already contains an entry for the key, update it's value and move it to the front of the list
                if (cache.TryGetValue(key, out node))
                {
                    list.Remove(node);
                    list.AddLast(node);

                    node.Value.Value = value;
                    return null;
                }

                // cache does not contain a value for key, so add it now
                else
                {
                    var cacheItem = new LRUCacheItem<TKey, TValue>(key, value);
                    node = new LinkedListNode<LRUCacheItem<TKey, TValue>>(cacheItem);

                    cache.Add(key, node);
                    list.AddLast(node);

                    LRUCacheItem<TKey, TValue> evicted = null;
                    if (cache.Count > capacity)
                        evicted = EvictLRU();
                    return evicted;
                }
            }
        }

        /// <summary>
        /// Evicts the least recently used cache item
        /// </summary>
        /// <returns>Returns the value of the item that was evicted</returns>
        LRUCacheItem<TKey, TValue> EvictLRU()
        {
            var lru = list.First;

            cache.Remove(lru.Value.Key);
            list.Remove(lru);

            return lru.Value;
        }




        #region IEnumerable  implementation

        public IEnumerator<LRUCacheItem<TKey, TValue>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion




        #region Partial Collection implementation

        public int Count
        {
            get { return list.Count; }
        }

        public void Clear()
        {
            lock (sync)
            {
                list.Clear();
                cache.Clear();
            }
        }

        #endregion




        #region Partical IDictionary<TKey, TValue> implementation

        public ICollection<TKey> Keys
        {
            get { return cache.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return cache.Values.Select(o => o.Value.Value).ToList(); }
        }

        // Summary:
        //     Determines whether the LRUCache<TKey,TValue>
        //     contains an element with the specified key.
        //
        // Parameters:
        //   key:
        //     The key to locate in the LRUCache<TKey,TValue>.
        //
        // Returns:
        //     true if the LRUCache<TKey,TValue> contains
        //     an element with the key; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     key is null.
        public bool ContainsKey(TKey key)
        {
            return cache.ContainsKey(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the LRUCache&lt;TKey,TValue&gt;.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false. This method
        /// also returns false if key was not found in the original LRUCacheLRUCache&lt;TKey,TValue&gt;.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// key is null.
        /// </exception>
        public bool Remove(TKey key)
        {
            lock (sync)
            {
                if (cache.ContainsKey(key))
                {
                    var node = cache[key];

                    list.Remove(node);
                    return cache.Remove(key);
                }
                else
                    return false;
            }
        }

        #endregion
    }
}