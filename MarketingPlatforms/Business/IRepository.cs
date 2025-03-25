namespace MarketingPlatforms.Business
{
    public interface IRepository<T> where T : class
    {
        bool TrySetValue(T data);
        T? GetDataByKey(string key);
    }
}
