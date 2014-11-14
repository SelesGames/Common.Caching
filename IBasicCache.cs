
namespace Common.Caching
{
    public interface IBasicCache<TKey, TResult>
    {
        TResult Get(TKey key);
    }
}